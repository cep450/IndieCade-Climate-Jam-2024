using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class SimVehicleType : Resource
{

	/*
	 *  Flyweight pattern: information about a particular type of vehicle, that multiple vehicles will use.
	 */

	// Infrastructure can have certain types of vehicles on it. 
	// e.g. 2 different types of buses both have the type bus, and can move on bus lanes and on roads.
	// bitmask enum
[System.Serializable]
[System.Flags]
	public enum TransportMode
	{
		PEDESTRIAN = 0x01,	// by foot. walk on sidewalks and other paths
		CAR = 0x02,			// single occupancy road vehicles. drive on roads, use parking lots
		BIKE = 0x04,		// single occupancy cycling/scooter/ect options. drive on bike lanes, use bike racks 
	// add new tags here
	//BUS = 0x08,	
	//TRAIN = 0x10,
	//F = 0x20,
	//G = 0x40
	//H = 0x80,
	//I = 0x100,
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
	public static int NumTransportModes { get {
		return Enum.GetNames(typeof(TransportMode)).Length;
	}}
	public static int ModeIndex(TransportMode mode) {
		return (int)Math.Log2((double)((int)mode));
	}

		// resources in order of enum values
	public static SimVehicleType [] types = {
		GD.Load<SimVehicleType>("res://Resources/VehicleTypes/pedestrian.tres"),
		GD.Load<SimVehicleType>("res://Resources/VehicleTypes/car.tres"),
		GD.Load<SimVehicleType>("res://Resources/VehicleTypes/bike.tres")
	};

	//TODO since this works the same as in SimInfraType, make it DRY after the jam? unless we can't since it uses different types.
	public static SimVehicleType TypeFromEnum(TransportMode enumType) {
		return types[ModeIndex(enumType)];
	}

	public static Godot.Collections.Array<SimVehicleType> TypesFromEnum(TransportMode enumType) {
		Godot.Collections.Array<SimVehicleType> arr = new Godot.Collections.Array<SimVehicleType>();
		for(int i = 0; i < types.Length; i++) {
			int bit = (int)Math.Pow(2, i);
			if((enumType & (TransportMode)bit) > 0) {
				arr.Add(types[i]);
			}
		}
		return arr;
	}

	[Export] public string name; // used by the game ui
	[Export] private TransportMode transportMode;
	[Export] private float emissionsPerYear;
	//private float emissionsPerTick; // derived from per year based on time scale in Sim
	[Export] private float maxSpeed;	// in miles per hour for familiarity and being able to look up data. In the future we can display this in different units by converting the number the UI displays, even if the internal number is the same.
	[Export] private int agentCapacity = 1; // how many agents can travel in it at a time
	[Export] public string visualVehicle;

	public TransportMode Mode { get => transportMode; private set {} }	// Determines what type/s of infrastructure it can travel on. 
	public float SpeedFactor { get => maxSpeed; private set {} }	// Maximum speed.
	public float Emissions { get {	// Emissions per tick when in use. Derived from emissions/year based on time scale.
		return emissionsPerYear / SimClock.ticksPerGameYear; //TODO find a more efficient way where we only calculate this once 
	} private set {} }	
	public HashSet<SimEdge.TransportMode> SupportedEdges { get; private set; }

}
