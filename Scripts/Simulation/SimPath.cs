using Godot;
using System;
using System.Collections.Generic;

public partial class SimPath : Node
{

	/*
	 *  An object representing a path between two tiles.
	 */
	
	// these lists are parallel
	public List<PathVertex> vertices;
	public List<PathEdge> edges; 
	public List<SimVehicleType.TransportMode> modes; //TODO we can use this once we get transit mode transfer in

	// running totals of weights 
	public float totalWeight;
	public float totalSupport;

	//what kind of vehicle is being used on this path
	public SimVehicleType pathVehicleType;

	public SimPath() {
		vertices = new List<PathVertex>();
		edges = new List<PathEdge>();
	}

}
