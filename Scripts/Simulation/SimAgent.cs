using Godot;
using System;
using System.Collections.Generic;


public partial class SimAgent : Node
{
	private float happiness; //would every agent have a happiness value?
	private Vector2 currentPosition;
	private Vector2 targetPosition;
	private SimPath pathFinder;

	public SimVehicle Vehicle { get; private set; }

	public SimAgent(SimVehicle vehicle)
	{
		Vehicle = vehicle;
	}

	public override void _Ready()
	{
		pathFinder = GetNode<SimPath>("../SimPath"); //get a reference to the pathfinder
		SetRandomTarget();
	}

	public override void _Process(double delta)
	{
		UpdateAgent();
	}

	public void UpdateAgent()
	{
		Vehicle._Process(GetProcessDeltaTime());
	}

	private void SetRandomTarget()
	{
		int gridSize = 10; //example number 
		targetPosition = new Vector2(GD.RandRange(0, gridSize), GD.RandRange(0, gridSize));
	}
}




