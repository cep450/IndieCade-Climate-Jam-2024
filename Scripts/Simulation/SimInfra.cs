using Godot;
using System;


public partial class SimInfra : Node
{

	/*
	 * A single instance of infra in the simulation. Tiles can have multiple infra on them.
	 */

	//TODO remove after restructure 
	public enum InfraType { House, Work, Road, Sidewalk, BikeLane, BusLane, Rail }
	public InfraType Type { get; private set; }
	public float BaseWeight { get; private set; }


	public SimInfraType type; // stores data about what it is 
	//public float BaseWeight { get => type.baseWeight; private set; }



	public float GetBaseWeight()
	{
		return BaseWeight;
	}

}

