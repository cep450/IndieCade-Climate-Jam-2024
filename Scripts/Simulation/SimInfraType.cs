using Godot;
using System;

[GlobalClass]
public partial class SimInfraType : Resource
{

	/*
	 *	Information about a type of tile that many instances of this type will share. e.g. information about "bike lanes"
	 */

	// bitmask enum
[System.Serializable]
[System.Flags]
	public enum InfraType
	{
		NONE = 0x00,
		HOUSE = 0x01,
		WORK = 0x02,
		ROAD = 0x04,
		BIKELANE = 0x08,
		SIDEWALK = 0x10,
		STREETLAMP = 0x20,
		CROSSWALK = 0x40

    // add new tags here
    //BUSLANE = 0x80,
    //RAIL = 0x100,
    //J = 0x200,
    //K = 0x400,
    //L = 0x800,
    //M = 0x1000,
    //N = 0x2000,
    //O = 0x4000,
    //P = 0x8000,
    //Q = 0x10000
    // ...ect
	}

	public static readonly InfraType [] types = {}; //TODO put the resources we're using in here, in oder so the indices match with the enum values

	[Export] public InfraType type;
	[Export] public SimVehicleType.TransportMode transportModes; // what transport modes can use this infrastructure
	[Export] public int capacity;	// how many vehicles or agents of this type can be on this infrastructure? TODO do we want to be able to have different capacities for different types of vehicle modes? or maybe it's the vehicle types that control how much space they take up.
	[Export] public bool canTransfer = false; // can a transit type switch to a different transit type?
									 //I think each tile should be able to have a list of what transit types it can support, and then we can check if the agent's current transit type is in that list when pathfinding,
									 //instead of assigning a transit type to each tile. This way, we can have tiles that support multiple transit types, and we can have tiles that support no transit types.
									 //this is implemented in SimTiles
	[Export] public float maxSpeed;
	[Export] public float safety;

	[Export] public int costToBuild;
	[Export] public int costToDestroy;

}