using Godot;
using System;
using System.Dynamic;


public partial class SimInfra : Node
{

	/*
	 * A single instance of infra in the simulation. Tiles can have multiple infra on them.
	 */

	private SimInfraType type; // stores data about what it is 
	private int occupancy; // out of capacity, how many things are on this instance (agents, vehicles, ect, depending on what type of vehicle this infra can have on it)

	// properties. all of these will return things calculated based on other data
	public SimInfraType Type { get => type; private set {} }	// reference to the SimInfraType with all the data
	public SimInfraType.InfraType TypeEnum { get => type.type; private set {} } //enum corresponding to the SimInfraType 

	public SimVehicleType.TransportMode TransportModes { get => type.transportModes; private set {} }
	public float Safety {get => type.safety; private set {} }
	public float MaxSpeed { get => type.maxSpeed; private set {} }
	public int Capacity { get => type.capacity; private set {} }
	public int Occupancy { get => occupancy; set {}}

	// vehicles will handle speed and emissions 

	// agents will handle calculating weight of paths, since different agents might weight things differently (e.g. how much they value safety and when)

	//only allow increase in occupancy if it doesn't go over capacity. returns true if action was successful
	public bool TryAddOccupancy() {
		if(occupancy < Capacity) {
			occupancy++;
			return true;
		}
		return false;
	}
	public bool TryRemoveOccupancy() {
		if(occupancy > 0) {
			occupancy--;
			return true;
		}
		return false;
	}
}

