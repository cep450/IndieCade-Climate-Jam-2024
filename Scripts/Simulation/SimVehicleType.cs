using Godot;
using System;
using System.Collections.Generic;

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
		NONE = 0x00,
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

	[Export] public string name; // used by the game ui
	[Export] private TransportMode transportMode;
	[Export] private float emissionsPerTick;
	[Export] private float maxSpeed;
	[Export] private int agentCapacity = 1; // how many agents can travel in it at a time

	public TransportMode Mode { get => transportMode; private set {} }	// Determines what type/s of infrastructure it can travel on. 
	public float SpeedFactor { get => maxSpeed; private set {} }	// Maximum speed.
	public float Emissions { get => emissionsPerTick; private set {} }	// Emissions per tick when in use.
	public HashSet<SimEdge.TransportMode> SupportedEdges { get; private set; }

	public SimVehicleType(TransportMode mode, float speedFactor, float emissions, HashSet<SimEdge.TransportMode> supportedEdges)
	{
		Mode = mode;
		SpeedFactor = speedFactor;
		Emissions = emissions;
		SupportedEdges = supportedEdges;
	}
}
