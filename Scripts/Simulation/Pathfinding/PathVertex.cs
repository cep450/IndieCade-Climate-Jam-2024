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
	public List<PathVertex> Neighbors; // can/might connect to this vertex
	public List<SimTile> tiles; // tiles whose infrastructure will affect this vertex

	public SimInfraType.InfraType InfraAffectedBy { get; private set; }

	public SimInfraType.DestinationType DestinationType { get; private set; } // if this vertex is a destination 

	public List<PathEdge> Edges;

	public bool CanTransfer { get; private set; }

	// these are parallel with SimVehicleType.TransportMode and are for each transport mode 
	int[] capacity;
	int[] occupancy;

	public PathVertex(Vector2I pathGraphCoordinates, Vector2 worldPosition) {
		PathGraphCoordinates = pathGraphCoordinates;
		WorldPosition = worldPosition;
		CalculateType();
		Edges = new List<PathEdge>();
		Neighbors = new List<PathVertex>();

		int numModes = SimVehicleType.NumTransportModes;
		capacity = new int[numModes];
		occupancy = new int[numModes];
	}

	private void CalculateType() {
		int moduloX = PathGraphCoordinates.X % 2;
		int moduloY = PathGraphCoordinates.Y % 2;

		if(moduloX == 0 && moduloY == 0) {
			type = Type.CORNER;
		} if(moduloX == 1 && moduloY == 1) {
			type = Type.CENTER;
		} else {
			type = Type.BORDER;
		}
	}

	public void RecalculateInfra() {

		InfraAffectedBy = 0x0;
		DestinationType = 0x0;
		CanTransfer = false;
		capacity = new int[SimVehicleType.NumTransportModes];

		//infra affects this vertex based on the infra's patterns 
		foreach(SimTile tile in tiles) {
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

	public void RecalculateEdges(bool updateNeighbors = false) {

		//TODO

		//TODO factor in CanTransfer 
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