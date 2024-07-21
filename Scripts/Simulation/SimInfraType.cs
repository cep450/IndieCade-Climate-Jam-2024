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
		NOT_DESTINATION,
		HOME, 
		WORKPLACE,
		COMMERCIAL,
		THIRDSPACE
	}

	// resources in order of enum values
	public static SimInfraType [] types = {
		GD.Load<SimInfraType>("res://Resources/InfraTypes/house.tres"),
		GD.Load<SimInfraType>("res://Resources/InfraTypes/work.tres"),
		GD.Load<SimInfraType>("res://Resources/InfraTypes/commercial.tres"),
		GD.Load<SimInfraType>("res://Resources/InfraTypes/thirdspace.tres"),
		GD.Load<SimInfraType>("res://Resources/InfraTypes/road.tres"),
		GD.Load<SimInfraType>("res://Resources/InfraTypes/sidewalk.tres"),
		GD.Load<SimInfraType>("res://Resources/InfraTypes/crosswalk.tres"),
		GD.Load<SimInfraType>("res://Resources/InfraTypes/bikelane.tres"),
		GD.Load<SimInfraType>("res://Resources/InfraTypes/streetlamp.tres"),
		GD.Load<SimInfraType>("res://Resources/InfraTypes/tree.tres"),
		GD.Load<SimInfraType>("res://Resources/InfraTypes/parking_lot.tres"),
		GD.Load<SimInfraType>("res://Resources/InfraTypes/bike_rack.tres"),
	};

	[Export] public string Name;
	[Export] public InfraType type;
	[Export] public NodePath visualInfra;
	[Export] public DestinationType destinationType; // if this is a destination, what type is it?
	[Export] public SimVehicleType.TransportMode transportModes; // what transport modes can use this infrastructure?
	[Export] public bool connectsCorners;	// connects on corners between tiles
	[Export] public bool connectsBorders; 	// connects on borders between tiles 
	[Export] public bool connectsCenter; 	// connects from the center to its edges
	[Export] public bool affectsAllVertices; // infra applies its weight effects to all PathVertexes on the tile 
	[Export] public SimVehicleType.TransportMode connectionsBlockedBy;

	// For vehicle-holding tiles, capacity represents the number of vehicles of that type that the VERTEX affected by infrastructure can hold at a time. 
	// For destination tiles, capacity represents the number of agents the building (vertex) can fit, or for HOMEs, how many agents are added to the map when this infra is added or generates on the map
	[Export] public int capacity;
	[Export] public SimVehicleType.TransportMode canTransfer; // what transit types can switch between each other
									// can a transit type switch to a different transit type?
									 //I think each tile should be able to have a list of what transit types it can support, and then we can check if the agent's current transit type is in that list when pathfinding,
									 //instead of assigning a transit type to each tile. This way, we can have tiles that support multiple transit types, and we can have tiles that support no transit types.
									 //this is implemented in SimTiles
	[Export] public float maxSpeed;
	[Export] public float safety;

	[Export] public int costToBuild;
	[Export] public int costToDestroy;

	[Export] public InfraType incompatibilityMask; //1 = incompatible, 0 = compatible

	[Export] public string ModelPath;
	[Export] public int ModelVariantCount = 1;
	[Export] public bool ModelConnects;
	[Export] public bool ModelHasBase;
	
	[Export] public Texture2D Icon;


	//TODO better way to do this?
	public static SimInfraType TypeFromEnum(InfraType enumType) {
		int index = (int)Math.Log2((double)((int)enumType));
		return types[index];
	}

	public static Godot.Collections.Array<SimInfraType> TypesFromEnum(InfraType enumType) {
		Godot.Collections.Array<SimInfraType> arr = new Godot.Collections.Array<SimInfraType>();
		for(int i = 0; i < types.Length; i++) {
			int bit = (int)Math.Pow(2, i);
			if((enumType & (InfraType)bit) > 0) {
				arr.Add(types[i]);
			}
		}
		return arr;
	}
	
	public static Godot.Collections.Array<SimInfraType> GetTypes()
	{
		var godotArray = new Godot.Collections.Array<SimInfraType>();
		foreach (var type in types)
		{
			godotArray.Add(type);
		}
		return godotArray;
	}

	// If we want specific types or groups of types to have extra behavior, they can extend this class and override these functions, which are called when the infra is added to or removed from a tile.
	public virtual void AddedToTile(SimTile tile) {}
	public virtual void RemovedFromTile(SimTile tile) {}

}
