using Godot;
using System.Collections.Generic;

public partial class SimTile : Node
{

	/*
	 * A single tile in the simulation.
	 */
	/*
	Vector2Int coordinates;

	//List<SimEdge> edges; //TODO are these stored here or in infra?
	//List<SimInfra> infra;

	

   */

	//TODO remove these following restructure 
	public SimInfra Infra { get; private set; }
	public List<SimEdge> Edges { get; private set; }


	public List<SimInfra> infra;	// infra currently on this tile

	public Vector2 Position { get; private set; }
	public int Capacity { get; private set; }
	
	public int gCost;
	public int hCost;
	public SimTile parent;

	//each tile knows what index it is on the SimGrid
	public int gridX;
	public int gridY;

	public SimTile(Vector2 position, int capacity)
	{
		Position = position;
		Capacity = capacity;
		Edges = new List<SimEdge>();
	}

	public void SetInfra(SimInfra infra)
	{
		Infra = infra;
	}

	public void AddEdge(SimEdge edge)
	{
		Edges.Add(edge);
	}

	public int fCost() 
	{
		return gCost + hCost;
	}
}
