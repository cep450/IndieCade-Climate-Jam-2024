using Godot;
using System;
using System.Collections.Generic;


public partial class SimAgent : Node
{

	// tuning values for use when generating initial values 
	
	//TODO make agents weight different factors differently 

	bool canDrive;

	private float happiness; //would every agent have a happiness value?
	public SimVehicle Vehicle { get; private set; }

	// smooth in-world positions for moving tile to tile
	public Vector2 currentPosition;
	public Vector2 targetPosition;

	// coordinate position on grid for moving destination to destination 
	public Vector2I currentCoordinates;
	public Vector2I targetCoordinates;

	private SimPath pathFinder;

	private SimInfraType.DestinationType destinationType;

	//TODO should we instantiate these in a different way? or is visual agent the node instance?
	// Randomize properties for this agent when it first spawns. does it have access to a car? how does it weight different factors?
	public SimAgent(float nonDriverProbability, Vector2I coordinates)
	{
		canDrive = GD.Randf() > nonDriverProbability;
		currentCoordinates = coordinates;
		//TODO set Vehicle to pedestrian by default, we'll need a list to get this
		destinationType = Sim.Instance.GetTile(currentCoordinates.X, currentCoordinates.Y).DestinationType;
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

	// choose a destination type that's not the current type and not NOT_DESTINATION 
	void ChooseNewDestinationType() {

		int numTypes = Enum.GetNames(typeof(SimInfraType.DestinationType)).Length - 1;
		int shift = GD.RandRange(1, numTypes - 1);
		
		//GD C# doesn't have repeat/wrap so I have to implement it myself 
		int newTypeInt = (int)destinationType + shift;
		while(newTypeInt > numTypes) {
			newTypeInt -= numTypes;
		}

		destinationType = (SimInfraType.DestinationType)newTypeInt;
	}

	void MoveToNextTile() {
		//TODO change the vehicle type to match the connection vehicle type if needed 
		//var vehicleType = new SimVehicleType(SimVehicleType.TransportMode.CAR, 1.0f, 5.0f, new HashSet<SimEdge.TransportMode> { SimEdge.TransportMode.Road });
		//var vehicle = new SimVehicle(vehicleType, new Vector2(0, 0));
		//TODO update the previous and next tile capacities to account for this agent moving between them 
			//TODO if the next tile is full, wait on the current tile, and check again next tick 
	}

	// Calculate how much this agent weights this connection between 2 tiles.
	float WeightConnection(SimTile from, SimTile to, SimVehicleType.TransportMode transportMode) {

		//TODO add factors like safety and whatever trees might give you from infrastructure on the subsequent tile 
		
		//TODO consider the max speed of [whatever's smaller: the current vehicle or the infrastructure for that vehicle]

		//TODO consider emissions of the current vehicle 

		return 0f;
	}
}




