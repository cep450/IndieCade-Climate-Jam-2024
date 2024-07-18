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

	public SimTile(Vector2I coordinates, Vector2 worldPosition, GodotObject newVisualTile)
	{
		Coordinates = coordinates;
		WorldPosition = worldPosition;
		PathVertices = new PathVertex[3,3];
		InfraTypesMask = default(SimInfraType.InfraType);
		visualTile = newVisualTile;
		Infra = new List<SimInfra>();
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
	public bool AddInfra(SimInfraType type, bool bypassValidation = false, bool updateVisuals = true) {

		//validate if the infrastructure can be added here, and if the player has enough currency
		if(!bypassValidation) {
			if(!CanAffordToAddInfra(type) || !CanAddInfraType(type)) {
				return false;
			}

			// pay the cost 
			Sim.Instance.SupportPool.SpendSupport(type.costToBuild);
		}

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

		//add edges accordingly
		RecalculateEdges();

		// if this infra has any special behavior when added
		type.AddedToTile(this);

		//update/add it visually
		if(updateVisuals) {
			visualTile.Call("update_visuals");

			//TODO update tiles adjacent to this tile visually
		}

		//return if adding was successful
		return true;
	}

	//TODO what do we want to pass in here? an index in the list? a type? an instance? maybe overrides for all of these. one that takes a mask could even add/remove multiple at once.
	public bool RemoveInfra(SimInfraType type, bool bypassValidation = false, bool updateVisuals = true) {
		
		if(!bypassValidation) {
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
		}

		// since we know the tile has it, remove from the mask 
		InfraTypesMask ^= type.type;

		// remove destination type if this infra provided one 
		if(type.destinationType != SimInfraType.DestinationType.NOT_DESTINATION) {
			DestinationType = SimInfraType.DestinationType.NOT_DESTINATION;
		}

		//remove from list 
		Infra.RemoveAt(IndexOfInfra(type.type));

		//update any connections 
		RecalculateEdges();

		// if this infra has any special behavior when removed
		type.RemovedFromTile(this);

		//update/remove it visually 
		// this should also update other infrastructure on the tile visually
		if(updateVisuals) {
			visualTile.Call("update_visuals");

			//TODO update tiles adjacent to this tile visually-- make sure this happens in update_visuals maybe 
		}

		return true;
	}

	public Godot.Collections.Array<SimInfraType> CompatibleInfra() {

		// calculate a mask representing the compatible infrastructure 
		SimInfraType.InfraType compatible = 0x0;

		foreach(SimInfra type in Infra) {
			compatible |= type.TypeEnum;
		}

		compatible = ~compatible;

		// return an array of those infrastructure 
		return SimInfraType.TypesFromEnum(compatible);
	}

	// index in infra list. returns -1 if none 
	public int IndexOfInfra(SimInfraType.InfraType type) {

		if(!HasInfraType(type)) return -1;

		return Infra.FindIndex(x => x.Type.type == type);
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

		// update the infrastructure affecting each PathVertex, according to what's on the current tile and tiles around it, and how the infra connects
		// TODO maybe we need to have an export with like a 3x3 set of flags for what vertices the infra influences?
		// like how trees affect everything, but destinations only exist on the center 
		// & maybe each PathVertex can figure itself out based on the vertices it knows about 
		// PathVertex.RecalculateInfra(), PathVertex.RecalculateConnections 
		// using ConnectsCorners/Borders/Center 


		// calculate connections between the PathVertexes, keeping blocking in mind 

		// TODO 
	}

	//TODO I have no idea what this means, could someone make the names more descriptive and/or comment this? --Jaden 
	public int fCost() 
	{
		return gCost + hCost;
	}
	
	// For testing
	public void SayHi()
	{
		GD.Print("Hi from SimTile");
	}

}
