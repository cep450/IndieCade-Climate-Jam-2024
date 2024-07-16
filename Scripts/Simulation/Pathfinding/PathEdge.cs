using Godot;
using System;
using System.Collections.Generic;

public class PathEdge {

	/*
	 *	A pathfinding edge. 
	 */

	public PathVertex A, B;
	public SimVehicleType.TransportMode TransportMode;

	// weights 
	public readonly float Distance = 1f;
	// all but distance are determined by infrastructure
	public float MaxSpeed = 1;
	public float Safety = 0;
}