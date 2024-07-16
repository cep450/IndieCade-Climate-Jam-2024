using Godot;
using System;
using System.Collections.Generic;

//////////////////////////////
// Since we have art assets for sidewalks and bike lanes being on the same tiles as roads, 
// here's an implementation of edges as being from specific corners of tiles to other corners of the same tile, a destination, or an interchange.
// so they're within tiles instead of between tiles. 
// which sucks a bit, and if we revisit this project we should probably put bike lanes and pedestrian stuff on tiles adjacent to roads instead of on the road tiles 
// but this is what we'll do for the end of the jam. 

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

	public PathfindingGraph(int sizeX, int sizeY) {
		vertexGrid = new PathVertex[sizeX, sizeY];
		for(int x = 0; x < sizeX; x++) {
			for(int y = 0; y < sizeY; y++) {
				vertexGrid[x,y] = new PathVertex(new Vector2I(x, y), new Vector2(SimGrid.TILE_WORLD_SCALE / 2 * x, SimGrid.TILE_WORLD_SCALE / 2 * y));
				
				// give appropriate tiles references to this vertex in the appropriate location 
				//TODO 
			}
		}
	}

	public PathVertex GetVertex(Vector2I coordinates) {
		return vertexGrid[coordinates.X, coordinates.Y];
	}

	public PathVertex GetVertex(Vector2I coordsOfTile, Vector2I coordsWithinTile) {
		return GetVertex(GetVertexCoordinates(coordsOfTile, coordsWithinTile));
	}

	// tile coordinates to vertex at tile center coordinates 
	public int TileToVertexCoord(int tileCoord) {
		return ((tileCoord + 1) * 2) - 1;
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

	public List<SimTile> GetTilesWithVertex(int xcoord, int ycoord) {

		List<SimTile> tiles = new List<SimTile>();

		// vertex coordinates to tile coordinates 
		float x = ((xcoord + 1) / 2) - 1;
		float y = ((ycoord + 1) / 2) - 1;

		float remainderx = x % 1;
		float remaindery = y % 1;

		if(remainderx == 0f && remaindery == 0f) {
			//if they're both whole numbers it's a tile center
			tiles.Add(Sim.Instance.GetTile((int)x, (int)y));
		} else {
			if(remainderx != 0f && remaindery != 0f) {
				// if neither are whole numbers it's a corner 
				tiles.Add(Sim.Instance.GetTile(Mathf.FloorToInt(x), Mathf.FloorToInt(y)));
				tiles.Add(Sim.Instance.GetTile(Mathf.FloorToInt(x), Mathf.CeilToInt(y)));
				tiles.Add(Sim.Instance.GetTile(Mathf.CeilToInt(x), Mathf.FloorToInt(y)));
				tiles.Add(Sim.Instance.GetTile(Mathf.CeilToInt(x), Mathf.CeilToInt(y)));
			} else {
				// if only one is a whole number it's a border 
				if(remainderx == 0f) {
					tiles.Add(Sim.Instance.GetTile((int)x, Mathf.FloorToInt(y)));
					tiles.Add(Sim.Instance.GetTile((int)x, Mathf.CeilToInt(y)));
				} else if(remaindery == 0f) {
					tiles.Add(Sim.Instance.GetTile(Mathf.FloorToInt(x), (int)y));
					tiles.Add(Sim.Instance.GetTile(Mathf.CeilToInt(x), (int)y));
				}
			}
		}
		return tiles;
	}
}

