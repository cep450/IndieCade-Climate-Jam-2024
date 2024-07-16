using Godot;
using System;
using System.Collections.Generic;

public partial class SimTile : Node
{

	/*
	 * A single tile in the simulation.
	 */

	public List<SimInfra> Infra { get; private set; } // infrastructure instances currently on this tile
	public SimInfraType.InfraType InfraTypesMask { get; private set; }
	public List<SimEdge> Edges { get; private set; }
	public Vector2 Position { get; private set; }

	public int gCost;
	public int hCost;
	public SimTile parent;

	//each tile knows what index it is on the SimGrid
	public int gridX;
	public int gridY;

	//GD visual tile 
	GDScript visualTileScript = GD.Load<GDScript>("res://Scripts/View/visual_tile.gd");
	GodotObject visualTile;

	public SimTile(Vector2 position)
	{
		Position = position;
		Edges = new List<SimEdge>();
		InfraTypesMask = default(SimInfraType.InfraType);
		visualTile = (GodotObject)visualTileScript.New();
	}


	// load infrastructure on a tile based on a type mask
	public void AddInfraFromMask(SimInfraType.InfraType mask) {

		for(int i = 0; i < sizeof(uint); i++) {
			SimInfraType.InfraType bit = (SimInfraType.InfraType)Math.Pow(2, i);
			if((mask & bit) != 0) {
				AddInfra(SimInfraType.TypeFromEnum(bit));
			}
		}
	}

	// Add infrastructure to the tile.
	public bool AddInfra(SimInfraType type, bool bypassValidation = false) {

		//validate if the infrastructure can be added here, and if the player has enough currency
		if(!bypassValidation) {
			if(!CanAffordToAddInfra(type) || !CanAddInfraType(type)) {
				return false;
			}
		}

		// pay the cost 
		Sim.Instance.SupportPool.SpendSupport(type.costToBuild);

		//update the mask representing all the types on this tile 
		InfraTypesMask |= type.type;

		//instantiate new infrastructure
		SimInfra newInfra = new SimInfra(type);

		//add it to the list 
		Infra.Add(newInfra);

		//TODO add edges accordingly 

		//update/add it visually
		visualTile.Call("update_visuals");

		//return if adding was successful
		return true;
	}

	//TODO what do we want to pass in here? an index in the list? a type? an instance? maybe overrides for all of these. one that takes a mask could even add/remove multiple at once.
	public bool RemoveInfra(SimInfraType type) {
		
		//check if this tile has this infrastrcture 
		if(!HasInfraType(type.type)) {
			//tile does not have the infrastructure 
			return false;
		}

		//validate that we can afford to remove this 
		if(!CanAffordToDestroyInfra(type)) {
			// cannot afford to remove this
			return false;
		}

		// pay the cost 
		Sim.Instance.SupportPool.SpendSupport(type.costToDestroy);

		// since we know the tile has it, remove from the mask 
		InfraTypesMask ^= type.type;

		//TODO remove from list 
		//TODO update any connections 

		//update/remove it visually 
		visualTile.Call("update_visuals");

		return true;
	}

	// can this infrastructure to be added to this tile, given the infrastructure it already has?
	public bool CanAddInfraType(SimInfraType type) {

		// ex. types mask 0101
		// compatibility mask 0001: incompatible 
		// compat mask 1000: compatible

		return (InfraTypesMask & type.incompatibilityMask) != 0;
	}

	public bool HasInfraType(SimInfraType.InfraType type) {
		return (InfraTypesMask & type) != 0;
	}

	// does the player have enough currency to buy this infrastructure?
	public bool CanAffordToAddInfra(SimInfraType type) {
		return Sim.Instance.SupportPool.HaveEnoughSupport(type.costToBuild);
	}
	public bool CanAffordToDestroyInfra(SimInfraType type) {
		return Sim.Instance.SupportPool.HaveEnoughSupport(type.costToDestroy);
	}

	public void AddEdge(SimEdge edge)
	{
		Edges.Add(edge);
	}

	//TODO I have no idea what this means, could someone make the names more descriptive and/or comment this? --Jaden 
	public int fCost() 
	{
		return gCost + hCost;
	}
}
