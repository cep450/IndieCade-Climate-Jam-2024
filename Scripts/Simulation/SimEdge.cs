using Godot;
using System;
using System.Collections.Generic;

public partial class SimEdge : Node
{

	/* 
	 * A connection agents and vehicles can travel on between 2 tiles.
	 * Infrastruture can add these.
	 */

	//TODO split up between type and instance 

	// types of vehicles that can travel along this connection e.g. a road could support both cars and buses, a bus lane only supports buses

	//TODO where to calculate weights? since they could come from a lot of factors- the speed of the vehicle, the infrastructure on the tile contributing to safety...
	// do tiles have edges or do tiles have infrastructure which has edges?

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
		// Example safety factor, could be influenced by infrastructure, accidents, etc.
		return 1.0f;
	}
}