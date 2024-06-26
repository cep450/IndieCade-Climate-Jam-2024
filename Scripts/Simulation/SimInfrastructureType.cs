using Godot;
using System;

public partial class SimInfrastructureType : Node
{
	
	public int costToBuild;
	public int costToDestroy;

	SimWeights weights; // includes stuff like speed, safety 

    public bool canTransfer = false; // can a transit type switch to a different transit type?
	
	// connections 
	// safety value 
	
}
