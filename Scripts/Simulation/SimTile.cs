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
	
	public enum Direction {
		NORTH = 0,
		EAST = 1,
		SOUTH = 2,
		WEST = 3
	}
	public enum DirectionCombination {
		NORTH_SOUTH = 0,
		EAST_WEST = 1,
		NORTH_EAST = 2,
		NORTH_WEST = 3,
		SOUTH_EAST = 4,
		SOUTH_WEST = 5
	}
	public readonly DirectionCombination[][] directionCombinations;
	public readonly Vector2I[] DIRECTIONS = { Vector2I.Up, Vector2I.Right, Vector2I.Down, Vector2I.Left };
	public static Direction GetDirection(Vector2I v) {
		if(v.Equals(Vector2I.Up)) return Direction.NORTH;
		if(v.Equals(Vector2I.Right)) return Direction.EAST;
		if(v.Equals(Vector2I.Down)) return Direction.SOUTH;
		if(v.Equals(Vector2I.Left)) return Direction.WEST;
		return (Direction)404; //invalid
	}

	public List<SimInfra> Infra { get; private set; } // infrastructure instances currently on this tile
	public SimInfraType.InfraType InfraTypesMask { get; private set; }
	public SimInfraType.DestinationType DestinationType { get; private set; }
	//public List<SimEdge> Edges { get; private set; }
	public Vector2I Position { get; private set; }

	// Stores connections between the north, south, east, west borders of tile.
	// First array: by ConnectionEvaluationOrder index (3)
	// Second array: by DirectionCombination index (6)
	//public Connection[][] Connections { get; private set; }

	public int gCost;
	public int hCost;
	public SimTile parent;

	//each tile knows what index it is on the SimGrid
	public int gridX;
	public int gridY;

	//GD visual tile 
	GDScript visualTileScript = GD.Load<GDScript>("res://Scripts/View/visual_tile.gd");
	GodotObject visualTile;

	public SimTile(Vector2I position)
	{
		Position = position;
		//Connections = new 
		//Edges = new List<SimEdge>();
		InfraTypesMask = default(SimInfraType.InfraType);
		visualTile = (GodotObject)visualTileScript.New();
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

		// if this infra has any special behavior when added
		type.AddedToTile(this);

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

		// remove destination type if this infra provided one 
		if(type.destinationType != SimInfraType.DestinationType.NOT_DESTINATION) {
			DestinationType = SimInfraType.DestinationType.NOT_DESTINATION;
		}

		//TODO remove from list 
		//TODO update any connections 

		// if this infra has any special behavior when removed
		type.RemovedFromTile(this);

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

	//TODO I have no idea what this means, could someone make the names more descriptive and/or comment this? --Jaden 
	public int fCost() 
	{
		return gCost + hCost;
	}
}
