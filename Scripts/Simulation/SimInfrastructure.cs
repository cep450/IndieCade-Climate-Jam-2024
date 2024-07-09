using Godot;
using System;


public partial class SimInfrastructure : Node
{

	/*
	 * A single piece of infrastructure in the simulation.
	 */

	public enum InfrastructureType { Road, Sidewalk, BikeLane, BusLane, Rail }

	public InfrastructureType Type { get; private set; }
	public float BaseWeight { get; private set; }

	public SimInfrastructure(InfrastructureType type, float baseWeight)
	{
		Type = type;
		BaseWeight = baseWeight;
	}

	public float GetBaseWeight()
	{
		return BaseWeight;
	}
}

