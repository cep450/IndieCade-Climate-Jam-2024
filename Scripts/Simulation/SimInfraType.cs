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
		HOME = 0x01,	//TODO if we need more numbers in the future we can condense home/work/commercial/thirdspace into a single DESTINATION infra type and determine its type via the DESTINATIONTYPE enum. since we're limited to 32 bits in an enum
		WORK = 0x02,
		COMMERCIAL = 0x04,
		THIRDSPACE = 0x08,
		ROAD = 0x10,
		SIDEWALK = 0x20,
		CROSSWALK = 0x40, 
		BIKELANE = 0x80,
		STREETLAMP = 0x100, 
		TREE = 0x200,
		PARKINGLOT = 0x400,
		BIKERACK =  0x800

	// add new tags here
	// RAIL, BUSLANE, BUSSTOP, RAILSTATION...
	//M = 0x1000,
	//N = 0x2000,
	//O = 0x4000,
	//P = 0x8000,
	//Q = 0x10000
	// ...ect
	}

	//destination type, for choosing where to pathfind 
	public enum DestinationType {
		HOME, 
		WORKPLACE,
		COMMERCIAL,
		THIRDSPACE
	}

	public static readonly SimInfraType [] types = {}; //TODO put the resources we're using in here, in oder so the indices match with the enum values

	[Export] public InfraType type;
	[Export] public SimVehicleType.TransportMode transportModes; // what transport modes can use this infrastructure?
	[Export] public DestinationType destinationType; // if this is a destination, what type is it?
	[Export] public int capacity;	// how many vehicles or agents of this type can be on this infrastructure? TODO do we want to be able to have different capacities for different types of vehicle modes? or maybe it's the vehicle types that control how much space they take up.
	[Export] public bool canTransfer = false; // can a transit type switch to a different transit type?
									 //I think each tile should be able to have a list of what transit types it can support, and then we can check if the agent's current transit type is in that list when pathfinding,
									 //instead of assigning a transit type to each tile. This way, we can have tiles that support multiple transit types, and we can have tiles that support no transit types.
									 //this is implemented in SimTiles
	[Export] public float maxSpeed;
	[Export] public float safety;

	[Export] public int costToBuild;
	[Export] public int costToDestroy;

	[Export] public InfraType incompatibilityMask; //1 = incompatible, 0 = compatible


	//TODO better way to do this?
	public static SimInfraType TypeFromEnum(InfraType enumType) {
		int index = (int)Math.Log2((double)((int)enumType));
		return types[index];
	}

}