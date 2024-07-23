using Godot;
using System;
using System.Collections.Generic;

//////////////////////////////
// Since we have art assets for sidewalks and bike lanes being on the same tiles as roads, 
// here's an implementation of edges as being from specific corners of tiles to other corners of the same tile, a destination, or an interchange.
// so they're within tiles instead of between tiles. 
// which sucks a bit, and if we revisit this project we should probably put bike lanes and pedestrian stuff on tiles adjacent to roads instead of on the road tiles 
// but this is what we'll do for the end of the jam. 

//TODO we probably want to make this a sparse graph 

public class PathfindingGraph {

	/*
		x = xertex 
		| or -- = edge 

		a single tile:
		x--x--x
		|  |  |
		x--x--x
		|  |  |
		x--x--x

		4 tiles in a 2x2 grid:
		x--x--x--x--x
		|  |  |  |  |
		x--x--x--x--x
		|  |  |  |  |
		x--x--x--x--x
		|  |  |  |  |
		x--x--x--x--x
		|  |  |  |  |
		x--x--x--x--x

		vertices are centers of tiles, corners where tiles meet, and borders between tiles 
		tiles share corners and borders 
	*/
	
	private PathVertex[,] vertexGrid; // and each of these will store its edges 
	int sizeX;
	int sizeY;
	public int Width { get => sizeX; }
	public int Height { get => sizeY; }


	public PathfindingGraph(int gridSizeX, int gridSizeY) {
		sizeX = TileToVertexCoord(gridSizeX) + 1;
		sizeY = TileToVertexCoord(gridSizeY) + 1;
		vertexGrid = new PathVertex[sizeX, sizeY];
		for(int vx = 0; vx < sizeX; vx++) {
			for(int vy = 0; vy < sizeY; vy++) {
				vertexGrid[vx,vy] = new PathVertex(new Vector2I(vx, vy), VertexToWorldCoord(vx, vy), this);
			}
		}

		// update tiles with the vertices relevant to them 
		for(int tx = 0; tx < gridSizeX; tx++) {
			for(int ty = 0; ty < gridSizeY; ty++) {

				SimTile tile = Sim.Instance.GetTile(tx, ty);
				int vx = TileToVertexCoord(tx);
				int vy = TileToVertexCoord(ty);

				tile.PathVertices[0,0] = vertexGrid[vx - 1, vy - 1];
				tile.PathVertices[0,1] = vertexGrid[vx - 1, vy];
				tile.PathVertices[0,2] = vertexGrid[vx - 1, vy + 1];

				tile.PathVertices[1,0] = vertexGrid[vx, vy - 1];
				tile.PathVertices[1,1] = vertexGrid[vx, vy];
				tile.PathVertices[1,2] = vertexGrid[vx, vy + 1];

				tile.PathVertices[2,0] = vertexGrid[vx + 1, vy - 1];
				tile.PathVertices[2,1] = vertexGrid[vx + 1, vy];
				tile.PathVertices[2,2] = vertexGrid[vx + 1, vy + 1];
			}
		}

		RecalculateAllVertices();
	}

	public void RecalculateAllVertices() {
		for(int vx = 0; vx < sizeX; vx++) {
			for(int vy = 0; vy < sizeY; vy++) {
				vertexGrid[vx,vy].RecalculateInfra();
			}
		}
		for(int vx = 0; vx < sizeX; vx++) {
			for(int vy = 0; vy < sizeY; vy++) {
				vertexGrid[vx,vy].RecalculateEdges();
			}
		}
	}

	public PathVertex GetVertex(Vector2I coordinates) {
		if(!CoordsInRange(coordinates.X, coordinates.Y)) return null;
		return vertexGrid[coordinates.X, coordinates.Y];
	}
	public PathVertex GetVertex(int x, int y) {
		if(!CoordsInRange(x, y)) return null;
		return vertexGrid[x,y];
	}

	public bool CoordsInRange(int x, int y) {
		return x >= 0 && y >= 0 && x < sizeX && y < sizeY;
	}

	public PathVertex GetVertex(Vector2I coordsOfTile, Vector2I coordsWithinTile) {
		return GetVertex(GetVertexCoordinates(coordsOfTile, coordsWithinTile));
	}

	// tile coordinates to vertex at tile center coordinates 
	public static int TileToVertexCoord(int tileCoord) {
		//GD.Print("tile: " + tileCoord.ToString() + " vertext: " + (((tileCoord + 1) * 2) - 1));
		return ((tileCoord + 1) * 2) - 1;
	}

	public static float VertexToTileCoord(int vertexCoord) {
		return (((float)vertexCoord + 1f) / 2f) - 1f;
	}

	public static Vector2 VertexToWorldCoord(int vertexX, int vertexY) {
		float gridx = VertexToTileCoord(vertexX);
		float gridy = VertexToTileCoord(vertexY);
		return Sim.Instance.grid.GridToWorldPos(gridx, gridy);
	}

	// coordsWithinTile: 0,0 means tile center, -1,-1 means bottom left corner, 1,1 means top right corner, ect 
	public Vector2I GetVertexCoordinates(Vector2I coordsOfTile, Vector2I coordsWithinTile) {

		// tile coordinates to vertex at tile center coordinates 
		int x = TileToVertexCoord(coordsOfTile.X);
		int y = TileToVertexCoord(coordsOfTile.Y);

		// then add coordinates within tile offset to get specific vertex 
		x += coordsWithinTile.X;
		y += coordsWithinTile.Y;

		return new Vector2I(x, y);
	}

// does not work as intended right now? might be fixed when I fixed VertexToTileCoord 
/*
	public List<SimTile> GetTilesWithVertex(int xcoord, int ycoord) {

		List<SimTile> tiles = new List<SimTile>();

		// vertex coordinates to tile coordinates 
		float x = VertexToTileCoord(xcoord);
		float y = VertexToTileCoord(ycoord);

		float remainderx = x % 1;
		float remaindery = y % 1;

		GD.Print("remainderx " + remainderx + " remaindery " + remaindery);

		if(remainderx == 0f && remaindery == 0f) {
			//if they're both whole numbers it's a tile center
			tiles.Add(Sim.Instance.GetTile((int)x, (int)y));
			GD.Print("both remainder 0");
		} else {
			if(remainderx != 0f && remaindery != 0f) {
				// if neither are whole numbers it's a corner 
				tiles.Add(Sim.Instance.GetTile(Mathf.FloorToInt(x), Mathf.FloorToInt(y)));
				tiles.Add(Sim.Instance.GetTile(Mathf.FloorToInt(x), Mathf.CeilToInt(y)));
				tiles.Add(Sim.Instance.GetTile(Mathf.CeilToInt(x), Mathf.FloorToInt(y)));
				tiles.Add(Sim.Instance.GetTile(Mathf.CeilToInt(x), Mathf.CeilToInt(y)));
				GD.Print("neither zero ");
			} else {
				// if only one is a whole number it's a border 
				if(remainderx == 0f) {
					tiles.Add(Sim.Instance.GetTile((int)x, Mathf.FloorToInt(y)));
					tiles.Add(Sim.Instance.GetTile((int)x, Mathf.CeilToInt(y)));
					GD.Print("x zero");
				} else if(remaindery == 0f) {
					tiles.Add(Sim.Instance.GetTile(Mathf.FloorToInt(x), (int)y));
					tiles.Add(Sim.Instance.GetTile(Mathf.CeilToInt(x), (int)y));
					GD.Print("y zero ");
				}
			}
		}
		GD.Print("tiles length was " + tiles.Count);
		return tiles;
	} */

	// since edges between the same verts with the same transport type are considered the same (and we're going to enforce that there's only one of them)
	public PathEdge GetEdge(PathVertex a, PathVertex b, SimVehicleType.TransportMode transportMode) {
		foreach(PathEdge e in a.Edges) {
			if(e.TransportMode == transportMode) {
				if(e.A.PathGraphCoordinates == a.PathGraphCoordinates) {
					if(e.B.PathGraphCoordinates == b.PathGraphCoordinates) {
						return e;
					}
				} else if(e.B.PathGraphCoordinates == a.PathGraphCoordinates) {
					if(e.A.PathGraphCoordinates == b.PathGraphCoordinates) {
						return e;
					}
				}
			}
		}
		return null;
	}

	// Add edge to the graph, or if it already exists, add its information to the existing edge. Returns the edge created or added to.
	public PathEdge AddEdge(PathVertex a, PathVertex b, SimVehicleType.TransportMode transportMode, float maxSpeed, float changeInSafety) {

		// if it already exists, add the info to the existing one
		PathEdge e = GetEdge(a, b, transportMode);
		if(e != null) {
			e.TransportMode |= transportMode;
			e.MaxSpeed = (MathF.Max(maxSpeed, e.MaxSpeed)); //TODO do we want to take the maximum of either? TODO do we want different infra to increase or decrease the max speed?
		} else {
			e = new PathEdge(a, b, transportMode, maxSpeed, changeInSafety);
			a.Edges.Add(e);
			b.Edges.Add(e);
		}

		return e;
	}

	// TODO Remove edge from a graph. If there's still info left over, we'll know the edge should stick around due to other infrastructure. TODO how to be sure though... like if there's 2 infra that both adds the same type of connection...
	// TODO we probably wont have this by the end of the jam, so, later
}

