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
	static bool DEBUG = false;

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
	public GodotObject VisualTile { get; private set; }

	public SimTile(Vector2I coordinates, Vector2 worldPosition, GodotObject newVisualTile)
	{
		Coordinates = coordinates;
		WorldPosition = worldPosition;
		PathVertices = new PathVertex[3,3];
		InfraTypesMask = default(SimInfraType.InfraType);
		VisualTile = newVisualTile;
		Infra = new List<SimInfra>();
	}

	// load infrastructure on a tile based on a type mask
	public void AddInfraFromMask(SimInfraType.InfraType mask, bool bypassCost = false, bool bypassCompatibility = false, bool updateVisuals = true, bool recalculateEdges = true) {

		for(int i = 0; i < SimInfraType.NumTypes; i++) {
			SimInfraType.InfraType bit = (SimInfraType.InfraType)Math.Pow(2, i);
			if((mask & bit) != 0) {
				AddInfra(SimInfraType.TypeFromEnum(bit), bypassCost, bypassCompatibility, updateVisuals, recalculateEdges);
			}
		}
	}

	// Add infrastructure to the tile. Returns false if the infrastructure could not be added.
	public string AddInfra(SimInfraType type, bool bypassCost = false, bool bypassCompatibility = false, bool updateVisuals = true, bool recalculateEdges = true) {

		if(DEBUG) GD.Print("called SimTile.AddInfra to add type " + type.Name);

		//validate if the infrastructure can be added here, and if the player has enough currency
		if(!bypassCompatibility) {
			if(!CanAddInfraType(type)){
				return "Not Valid Space to Add";
			}

		}
		if(!bypassCost) {

			if(!CanAffordToAddInfra(type)){
				return "Can't Afford";
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
		if(recalculateEdges) RecalculateVertices();

		// if this infra has any special behavior when added
		type.AddedToTile(this);

		//update/add it visually
		if(updateVisuals) {
			VisualTile.Call("update_visuals");

			//TODO update tiles adjacent to this tile visually
		}

		//return "" adding was successful
		return "";
	}

	//TODO what do we want to pass in here? an index in the list? a type? an instance? maybe overrides for all of these. one that takes a mask could even add/remove multiple at once.
	public bool RemoveInfra(SimInfraType type, bool bypassCost = false, bool updateVisuals = true, bool recalculateEdges = true) {
		//check if this tile has this infrastrcture 
		if(!HasInfraType(type.type)) {
			//tile does not have the infrastructure 
			return false;
		}
		
		if(!bypassCost) { 

			//validate that we can afford to remove this 
			if(!CanAffordToDestroyInfra(type)) {
				// cannot afford to remove this
				return false;
			}

			// pay the cost 
			Sim.Instance.SupportPool.SpendSupport(type.costToDestroy);
		}

		// remove destination type if this infra provided one 
		if(type.destinationType != SimInfraType.DestinationType.NOT_DESTINATION) {
			DestinationType = SimInfraType.DestinationType.NOT_DESTINATION;
		}

		//remove from list 
		Infra.RemoveAt(IndexOfInfra(type.type));

		// since we know the tile has it, remove from the mask 
		InfraTypesMask ^= type.type;

		//update any connections 
		if(recalculateEdges) RecalculateVertices();

		// if this infra has any special behavior when removed
		type.RemovedFromTile(this);

		//update/remove it visually 
		// this should also update other infrastructure on the tile visually
		if(updateVisuals) {
			VisualTile.Call("update_visuals");

			//TODO update tiles adjacent to this tile visually-- make sure this happens in update_visuals maybe 
		}
		return true;
	}

	public Godot.Collections.Array<SimInfraType> CompatibleInfra() {

		// calculate a mask representing the compatible infrastructure 
		SimInfraType.InfraType compatible = 0x0;

		foreach(SimInfra type in Infra) {
			compatible |= type.Type.incompatibilityMask;
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

		if(DEBUG) GD.Print("is " + InfraTypesMask.ToString() + " compatible with the incompatibility mask " + type.incompatibilityMask.ToString() + "? " + ((InfraTypesMask & type.incompatibilityMask) == 0));

		return (InfraTypesMask & type.incompatibilityMask) == 0;
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
	public void RecalculateVertices() {

		// update the infrastructure affecting each PathVertex, according to what's on the current tile and tiles around it, and how the infra connects

		// update infrastructure on the vertices
		for(int x = 0; x < 3; x++) {
			for(int y = 0; y < 3; y++) {
				PathVertices[x,y]?.RecalculateInfra();
			}
		}

		// now that we know the infrastructure, update the connections on these vertices 
		for(int x = 0; x < 3; x++) {
			for(int y = 0; y < 3; y++) {
				PathVertices[x,y]?.RecalculateEdges();
			}
		}

		// TODO maybe we need to have an export with like a 3x3 set of flags for what vertices the infra influences?
		// like how trees affect everything, but destinations only exist on the center 
	}

	//TODO I have no idea what this means, could someone make the names more descriptive and/or comment this? --Jaden 
	public int fCost() 
	{
		return gCost + hCost;
	}
}
