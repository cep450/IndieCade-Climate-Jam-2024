using Godot;
using System;

public partial class SimClock : Node
{
	
	const float secondsPerTick = 1f;

	double counter = 0f;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		counter += delta;
		//TODO determine if we need to call the next tick, and account for if the last tick has completed (so we don't skip ticks if we're lagging)
	}

	// use godot signals to send signals to objects that a tick has happened 

}
