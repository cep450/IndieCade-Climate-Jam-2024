using Godot;
using System.Collections.Generic;

public partial class SimTile : Node
{

	/*
	 * A single tile in the simulation.
	 */

	public List<SimInfra> Infra { get; private set; } // infrastructure instances currently on this tile
	public List<SimEdge> Edges { get; private set; }

	public Vector2 Position { get; private set; }
	public int Capacity { get; private set; } //TODO refactor to use the capacity of individual infrastructure on the tile instead of the tile having capacity
	
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

	// Add infrastructure to the tile.
	public bool AddInfra(SimInfraType type, bool bypassValidation = false) {

		//validate if the infrastructure can be added here, and if the player has enough currency
		if(!bypassValidation) {
			if(!CanAffordToAddInfra(type) || !CanAddInfraType(type)) {
				return false;
			}
		}

		//TODO instantiate new infrastructure
		//TODO add it to the list 
		//TODO add edges accordingly 
		//TODO update/add it visually

		//return if adding was successful
		return true;
	}

	public void RemoveInfra() {
		//TODO what do we want to pass in here? an index in the list? a type? an instance?
	}

	// can this infrastructure to be added to this tile, given the infrastructure it already has?
	public bool CanAddInfraType(SimInfraType type) {
		//TODO
		return true;
	}

	// does the player have enough currency to buy this infrastructure?
	public bool CanAffordToAddInfra(SimInfraType type) {
		//TODO 
		return true;
	}

	public void AddEdge(SimEdge edge)
	{
		Edges.Add(edge);
	}

	// Based on the infrastructure on this tile and the tiles around it, update its appearance. 
	public void UpdateVisualTile() {
		//TODO
	}

	//TODO I have no idea what this means, could someone make the names more descriptive and/or comment this? --Jaden 
	public int fCost() 
	{
		return gCost + hCost;
	}
}
