using Godot;
using System;

[GlobalClass]
public partial class SimInfraType : Resource
{

	/*
	 *	Information about a type of tile that many instances of this type will share. e.g. information about "bike lanes"
	 */
	[Export] public int costToBuild;
	[Export] public int costToDestroy;

	SimWeights weights; // includes stuff like speed, safety 

	public bool canTransfer = false; // can a transit type switch to a different transit type?
									 //I think each tile should be able to have a list of what transit types it can support, and then we can check if the agent's current transit type is in that list when pathfinding,
									 //instead of assigning a transit type to each tile. This way, we can have tiles that support multiple transit types, and we can have tiles that support no transit types.
									 //this is implemented in SimTiles
	// connections 
	// safety value 

}