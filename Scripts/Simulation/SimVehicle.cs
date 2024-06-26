using Godot;
using System;

public partial class SimVehicle : Node
{
	/* 
	 *	A single vehicle.
	 */

	public SimVehicleType type;

	
	bool inUse;	// emissions are only created if the vehicle is in use

	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
