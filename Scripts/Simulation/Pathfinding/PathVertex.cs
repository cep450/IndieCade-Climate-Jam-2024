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

	// stores an in-world location, so that agents can travel between vertices visually 
	public Vector2 WorldPosition { get; private set; }
	public Vector2I PathGraphCoordinates {get; private set; }

	public SimInfraType.DestinationType DestinationType { get; private set; } // if this vertex is a destination 

	public List<PathEdge> Edges;

	public bool canTransfer = false;

	public PathVertex(Vector2I pathGraphCoordinates, Vector2 worldPosition) {
		PathGraphCoordinates = pathGraphCoordinates;
		WorldPosition = worldPosition;
		Edges = new List<PathEdge>();
	}

	//TODO we might want capacity and occupancy represented here, if we represent this at all before the jam 

	//TODO maybe infrastructure has these. sets these up on the tiles
	// like a tile could have a pedestrian east, pedestrian west, car north, car south...
}