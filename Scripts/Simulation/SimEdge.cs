using Godot;
using System;
using System.Collections.Generic;

public class SimEdge {

	/* 
	 * A connection agents and vehicles can travel on between 2 tiles.
	 * Infrastruture can add these.
	 */

	 //TODO split up between type and instance 

	// types of vehicles that can travel along this connection e.g. a road could support both cars and buses, a bus lane only supports buses
	List<Sim.TransitType> canTravel;

	//TODO where to calculate weights? since they could come from a lot of factors- the speed of the vehicle, the infrastructure on the tile contributing to safety...
	// do tiles have edges or do tiles have infrastructure which has edges?


}
