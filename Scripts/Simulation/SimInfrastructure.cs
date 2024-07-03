using Godot;
using System;


public partial class SimInfrastructure : Node
{

	/*
	 * A single instance of infrastructure in the simulation. Tiles can have multiple infrastructure on them.
	 */

	//TODO remove after restructure 
	public enum InfrastructureType { House, Work, Road, Sidewalk, BikeLane, BusLane, Rail }
	public InfrastructureType Type { get; private set; }
	public float BaseWeight { get; private set; }


	public SimInfrastructureType type; // stores data about what it is 
	//public float BaseWeight { get => type.baseWeight; private set; }



	public float GetBaseWeight()
	{
		return BaseWeight;
	}

}

