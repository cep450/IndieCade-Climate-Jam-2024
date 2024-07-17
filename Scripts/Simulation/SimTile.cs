using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class SimTile : Node
{

	/*
	 * A single tile in the simulation.
	 */

	public List<SimInfra> Infra { get; private set; } // infrastructure instances currently on this tile
	public SimInfraType.InfraType InfraTypesMask { get; private set; }
	public SimInfraType.DestinationType DestinationType { get; private set; }
	public Vector2I Coordinates { get; private set; } // coordinates on the SimGrid
	public Vector2 WorldPosition { get; private set; } 
	public PathVertex[,] PathVertices;

	public int gCost;
	public int hCost;
	public SimTile parent;

	//GD visual tile 
	GDScript visualTileScript = GD.Load<GDScript>("res://Scripts/View/view_tile.gd");
	GodotObject visualTile;

	public SimTile(Vector2I coordinates, Vector2 worldPosition)
	{
		Coordinates = coordinates;
		WorldPosition = worldPosition;
		PathVertices = new PathVertex[3,3];
		InfraTypesMask = default(SimInfraType.InfraType);
		visualTile = (GodotObject)visualTileScript.New();
		//TODO move visual tile to worldposition 
	}

	// load infrastructure on a tile based on a type mask
	public void AddInfraFromMask(SimInfraType.InfraType mask, bool bypassValidation) {

		for(int i = 0; i < sizeof(uint); i++) {
			SimInfraType.InfraType bit = (SimInfraType.InfraType)Math.Pow(2, i);
			if((mask & bit) != 0) {
				AddInfra(SimInfraType.TypeFromEnum(bit), bypassValidation);
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

		// update the destination type of the tile
		if(type.destinationType != SimInfraType.DestinationType.NOT_DESTINATION) {
			DestinationType = type.destinationType;
		}

		//instantiate new infrastructure
		SimInfra newInfra = new SimInfra(type);

		//add it to the list 
		Infra.Add(newInfra);

		//TODO add edges accordingly
		RecalculateEdges();

		// if this infra has any special behavior when added
		type.AddedToTile(this);

		//update/add it visually
		visualTile.Call("update_visuals");

		//TODO update OTHER infrastructure on the tile visually

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

		// remove destination type if this infra provided one 
		if(type.destinationType != SimInfraType.DestinationType.NOT_DESTINATION) {
			DestinationType = SimInfraType.DestinationType.NOT_DESTINATION;
		}

		//TODO remove from list 
		//Infra.Remove() //TODO how to find it?

		//TODO update any connections 
		RecalculateEdges();

		// if this infra has any special behavior when removed
		type.RemovedFromTile(this);

		//update/remove it visually 
		visualTile.Call("update_visuals");

		//TODO update OTHER infrastructure on the tile visually

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

	// Reclaculate the edges to/from the vertices on this tile based on the current lineup of infrastructure on the tile and around the tile.
	void RecalculateEdges() {
		// TODO 
	}

	//TODO I have no idea what this means, could someone make the names more descriptive and/or comment this? --Jaden 
	public int fCost() 
	{
		return gCost + hCost;
	}
}