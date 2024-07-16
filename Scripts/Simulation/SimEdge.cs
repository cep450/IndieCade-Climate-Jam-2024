using Godot;
using System;
using System.Collections.Generic;

public partial class SimEdge : Node
{

	/* 
	 * A connection agents and vehicles can travel on between 2 tiles.
	 * Infrastruture can add these.
	 * Used for pathfinding.
	 */

	//TODO split up between type and instance 

	// types of vehicles that can travel along this connection e.g. a road could support both cars and buses, a bus lane only supports buses

	//TODO where to calculate weights? since they could come from a lot of factors- the speed of the vehicle, the infrastructure on the tile contributing to safety...
	// do tiles have edges or do tiles have infrastructure which has edges?


	// SimEdges will store factors that influence weight individually, since the ultimate weight is calculated by the agent, who decides what to value.
	// Agents will value factors depending on their values and on their current transportation mode- e.g. pedestrians and cyclists will value trees and crosswalks for safety much more than those in cars
	// or maybe infra decides who values it? like only like transportation modes influence values 
	// or an additional mask for what transport modes value this
	
	public enum TransportMode { Road, Sidewalk, BikeLane, BusLane, Rail }

	public Vector2 StartNode { get; private set; }
	public Vector2 EndNode { get; private set; }
	public TransportMode Mode { get; private set; }
	public float BaseWeight { get; private set; }
	public HashSet<TransportMode> SupportedModes { get; private set; }
	public Dictionary<TransportMode, float> Weights { get; private set; }


	public SimEdge(Vector2 startNode, Vector2 endNode, TransportMode mode, float baseWeight)
	{
		StartNode = startNode;
		EndNode = endNode;
		Mode = mode;
		BaseWeight = baseWeight;
		SupportedModes = new HashSet<TransportMode>();
		Weights = new Dictionary<TransportMode, float>();
	}

	public float CalculateWeight(TransportMode mode)
	{
		// Example calculation for weight based on transport mode and edge properties
		float speedFactor = GetSpeedFactor(mode);
		float safetyFactor = GetSafetyFactor();
		return BaseWeight / speedFactor * safetyFactor;
	}

	private float GetSpeedFactor(TransportMode mode)
	{
		// Example speed factors for different modes of transport
		switch (mode)
		{
			case TransportMode.Road: return 1.0f;
			case TransportMode.Sidewalk: return 0.5f;
			case TransportMode.BikeLane: return 1.2f;
			case TransportMode.BusLane: return 0.8f;
			case TransportMode.Rail: return 1.5f;
			default: return 1.0f;
		}
	}

	public float GetSafetyFactor()
	{
		// Example safety factor, could be influenced by infra, accidents, etc.
		return 1.0f;
	}

}





class PathfindingEdge {
	//////////////////////////////
	// Since we have art assets for sidewalks and bike lanes being on the same tiles as roads, 
	// here's an implementation of edges as being from specific corners of tiles to other corners of the same tile, a destination, or an interchange.
	// so they're within tiles instead of between tiles. 
	// which sucks a bit, and if we revisit this project we should probably put bike lanes and pedestrian stuff on tiles adjacent to roads instead of on the road tiles 
	// but this is what we'll do for the end of the jam. 

	public SimTile currentTile;
	public Vector2I coordinates;
	public PathfindingVertex A, B;

	public float maxSpeed;
	public float distance;
	public float safety;
}

class PathfindingVertex {

	// a tile corner, tile border, a destination, or an interchange 
	// stores connected edges by TransportationMode 
	// stores an in-world location, so that agents can travel between vertices visually 
		//TODO do we also want to set these up on tile model godot nodes so they actually walk on crosswalks and the like. probably after the jam 

	//TODO maybe infrastructure has these. sets these up on the tiles
	// like a tile could have a pedestrian east, pedestrian west, car north, car south...
}
