using Godot;
using System;
using System.Collections.Generic;

public class PathVertex {

	/*
	 *	A pathfinding vertex.
	 *  Represents a tile corner, a tile border, or a tile center 
	 *  Might represent a destination 
	 *  Might be an interchange if edges with multiple transport modes connect to it 
	 */

	public enum Type {
		CENTER, BORDER, CORNER
	}

	// stores an in-world location, so that agents can travel between vertices visually 
	public Vector2 WorldPosition { get; private set; }
	public Vector2I PathGraphCoordinates {get; private set; }
	public Type type {get; private set;}
	//public List<PathVertex> Neighbors; // can/might connect to this vertex
	public List<SimTile> Tiles; // tiles whose infrastructure will affect this vertex

	public SimInfraType.InfraType InfraAffectedBy { get; private set; }

	public SimInfraType.DestinationType DestinationType { get; private set; } // if this vertex is a destination 

	public List<PathEdge> Edges;

	public bool CanTransfer { get; private set; }

	// these are parallel with SimVehicleType.TransportMode and are for each transport mode 
	int[] capacity;
	int[] occupancy;

	PathfindingGraph pathGraph;

	public PathVertex(Vector2I pathGraphCoordinates, Vector2 worldPosition, PathfindingGraph graph) {
		PathGraphCoordinates = pathGraphCoordinates;
		WorldPosition = worldPosition;
		CalculateType();
		pathGraph = graph;
		//Tiles = pathGraph.GetTilesWithVertex(pathGraphCoordinates.X, pathGraphCoordinates.Y);
		FindTiles();
		Edges = new List<PathEdge>();
		//Neighbors = new List<PathVertex>();

		int numModes = SimVehicleType.NumTransportModes;
		capacity = new int[numModes];
		occupancy = new int[numModes];
	}

	private void CalculateType() {

		int moduloX = PathGraphCoordinates.X % 2;
		int moduloY = PathGraphCoordinates.Y % 2;

		if(moduloX == 0 && moduloY == 0) {
			type = Type.CORNER;
		} else if(moduloX == 1 && moduloY == 1) {
			type = Type.CENTER;
		} else {
			type = Type.BORDER;
		}

		//GD.Print("type was " + type + " modulox " + moduloX + " moduly " + moduloY);
	}

	void FindTiles() {

		Tiles = new List<SimTile>();

		// vertex coordinates to tile coordinates 
		float x = PathfindingGraph.VertexToTileCoord(PathGraphCoordinates.X);
		float y = PathfindingGraph.VertexToTileCoord(PathGraphCoordinates.Y);

		if(type == Type.CENTER) {
			AddTileIfNotNull((int)x, (int)y);
		} else {
			if(type == Type.CORNER) {
				AddTileIfNotNull(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
				AddTileIfNotNull(Mathf.FloorToInt(x), Mathf.CeilToInt(y));
				AddTileIfNotNull(Mathf.CeilToInt(x), Mathf.FloorToInt(y));
				AddTileIfNotNull(Mathf.CeilToInt(x), Mathf.CeilToInt(y));
			} else if(type == Type.BORDER){
				// it's a border
				// have to calculate if it's a border on the x or y 
				// or just use floor anyway since it'll always go to the same number if it's an int?
				AddTileIfNotNull(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
				AddTileIfNotNull(Mathf.CeilToInt(x), Mathf.CeilToInt(y));
				/* 
				if(remainderx == 0f) {
					Tiles.Add(Sim.Instance.GetTile((int)x, Mathf.FloorToInt(y)));
					Tiles.Add(Sim.Instance.GetTile((int)x, Mathf.CeilToInt(y)));
					GD.Print("x zero");
				} else if(remaindery == 0f) {
					Tiles.Add(Sim.Instance.GetTile(Mathf.FloorToInt(x), (int)y));
					Tiles.Add(Sim.Instance.GetTile(Mathf.CeilToInt(x), (int)y));
					GD.Print("y zero ");
				} */
			}
		}
		//GD.Print("tiles length was " + Tiles.Count);
	}

	void AddTileIfNotNull(int tx, int ty) {
		SimTile tile = Sim.Instance.GetTile(tx, ty);
		if(tile != null) {
			Tiles.Add(tile);
		}
	}

	public void RecalculateInfra() {

		InfraAffectedBy = 0x0;
		DestinationType = 0x0;
		CanTransfer = false;
		capacity = new int[SimVehicleType.NumTransportModes];
		Edges = new List<PathEdge>();

		//infra affects this vertex based on the infra's patterns 
		foreach(SimTile tile in Tiles) {
			//GD.Print();
			//GD.Print("tile was " + tile.ToString() + (tile == null).ToString());
			//GD.Print("infra was " + tile.Infra.ToString() + (tile.Infra == null).ToString());
			foreach(SimInfra infra in tile.Infra) {

				if(infra.Type.affectsAllVertices) {
					AddInfra(infra.Type);
				} else {
					if(type == Type.CENTER && infra.Type.connectsCenter) {
						AddInfra(infra.Type);
					} else if(type == Type.BORDER && infra.Type.connectsBorders || infra.Type.connectsCorners) {
						AddInfra(infra.Type);
					} else if(type == Type.CORNER && infra.Type.connectsCorners) {
						AddInfra(infra.Type);
					}
				}
			}
		}
	}

	private void AddInfra(SimInfraType infraType) {
		InfraAffectedBy |= infraType.type;
		DestinationType |= infraType.destinationType;
		CanTransfer = infraType.canTransfer;
		int numModes = SimVehicleType.NumTransportModes;

		for(int i = 0; i < numModes; i++) {
			int bit = (int)Math.Pow(2, i);
			if((infraType.transportModes & (SimVehicleType.TransportMode)bit) > 0) {
				capacity[i] = Mathf.Max(infraType.capacity, capacity[i]);
			}
		}
	}

	public void RecalculateEdges() {

		if(type == Type.CENTER || type == Type.CORNER) {
			// link up to alike infra normally 

		} else if(type == Type.BORDER) {
			// worry about road blocking 
			//TODO right now roads are the only blocking infra, but later we can make this generic. this solution is just for the jam

		} 

		//GD.Print("recalculating vertex " + PathGraphCoordinates.ToString());

		//TODO factor in blocking 

		// TODO factor in corner/border/center 

		// TODO factor in CanTransfer 

		// TODO add infra info to the edges like MaxSpeed 
	}

	void AddEdge(SimInfraType type) {

	}

	public int GetOccupancy(SimVehicleType.TransportMode mode) {
		return occupancy[SimVehicleType.ModeIndex(mode)];
	}
	public int GetCapacity(SimVehicleType.TransportMode mode) {
		return capacity[SimVehicleType.ModeIndex(mode)];
	}

	// handle capacity and occupancy per pathfinding vertex per transit mode 
	//only allow increase in occupancy if it doesn't go over capacity. returns true if action was successful
	public bool TryAddOccupancy(SimVehicleType.TransportMode mode) {
		int modeIndex = SimVehicleType.ModeIndex(mode);
		if(occupancy[modeIndex] < capacity[modeIndex]) {
			occupancy[modeIndex]++;
			return true;
		}
		return false;
	}
	public bool TryRemoveOccupancy(SimVehicleType.TransportMode mode) {
		int modeIndex = SimVehicleType.ModeIndex(mode);
		if(occupancy[modeIndex] > 0) {
			occupancy[modeIndex]--;
			return true;
		}
		return false;
	}
}
