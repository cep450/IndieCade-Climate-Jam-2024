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
	public SimVehicle Vehicle { get; private set; }

	// smooth in-world positions for moving tile to tile
	public Vector2 currentPosition;
	public Vector2 targetPosition;

	// coordinate position on grid for moving destination to destination 
	public Vector2Int currentCoordinates;
	public Vector2Int targetCoordinates;

	private SimPath pathFinder;

	private SimInfraType.DestinationType lastDestinationType;

	// Randomize properties for this agent when it first spawns. does it have access to a car? how does it weight different factors?
	public SimAgent(float _nonDriverProbability, Vector2Int coordinates)
	{
		//TODO generate driver yes/no 
		currentCoordinates = coordinates;
		pathFinder = GetNode<SimPath>("../SimPath"); //get a reference to the pathfinder
		SetRandomTarget();
	}

	// every game tick. update position smoothly
	public override void _Process(double delta)
	{
		Vehicle._Process(GetProcessDeltaTime());
	}

	// every simulation tick.
	public void Tick() {
		Vehicle.Tick();

		//TODO check if arrived at destination 
		// TODO if so, 
			// generate support based on the route 
			//ChooseDestinationType() and ChooseTarget() 
	}

	//TODO we'll be replacing this with ChooseTarget()
	private void SetRandomTarget()
	{
		targetPosition = new Vector2(GD.RandRange(0, Sim.Instance.grid.Width), GD.RandRange(0, Sim.Instance.grid.Height));
		Vehicle.SetTarget(targetPosition);
	}

	void ChooseDestinationType() {

	}

	void ChooseTarget() {

	}
}




