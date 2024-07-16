using Godot;
using System;
using System.Collections.Generic;


public partial class SimAgent : Node
{

	// tuning values for use when generating initial values 
	float nonDriverProbability = 0.3f; // does this agent have access to a car? TODO see if we can find this figure-- most immediately accessible statistics only measure adults, or households
	//TODO make agents weight different factors differently 

	bool canDrive;

	private float happiness; //would every agent have a happiness value?
	public Vector2 currentPosition;
	public Vector2 targetPosition;
	private SimPath pathFinder;

	public SimVehicle Vehicle { get; private set; }

	public SimAgent(SimVehicle vehicle)
	{
		Vehicle = vehicle;
	}

	public override void _Ready()
	{
		pathFinder = GetNode<SimPath>("../SimPath"); //get a reference to the pathfinder
		Generate();
		SetRandomTarget();
	}

	// Randomize properties for this agent when it first spawns. does it have access to a car? how does it weight different factors?
	public void Generate() {
		//TODO set canDrive based on nonDriverProbability 
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
		Vehicle.SetTarget(targetPosition);
	}
}




