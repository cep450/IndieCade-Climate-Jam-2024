using Godot;
using System;

public class SimAgent
{
	public float happiness;

    SimTile currentTile; // location 
	SimTile destination;
	SimPath path = null;

	SimWeights values; // how much this agent weighs various factors when pathfinding


}
