using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


public partial class SimAgent : Node
{

		// TODO generate these properly later
	float suppFactorSafety = 1f;
	float suppFactorDistance = -0.1f;
	float suppFactorEmissions = -0.1f;
									// pedestrian, car, bike
	float [] suppFactorsTransportMode = { 0.2f, 0f, 0.1f }; //TODO define this properly later

	
	// TODO generate these properly later
	float weightFactorSafety = -0.9f;
	float weightFactorDistance = 1f;
	float weightFactorEmissions = 0.5f;
									// pedestrian, car, bike
	float [] weightFactorsTransportMode = { 0.1f, 0.5f, 0.2f }; //TODO define this properly later

	float cantTakeTransitTypeWeightMod = 10f;



	public enum State {
		AT_DESTINATION, 
		TRAVELLING
	}
	public State state {get; private set; }

	//TODO if we want to make the pathfinding process non-blocking to avoid lag, we could have them pathfind while they're chilling at their location


	//TODO make agents weight different factors differently 

	bool canDrive;

	private float happiness; //would every agent have a happiness value?
	public SimVehicle Vehicle { get; private set; }
	public bool VehicleIsInUse { get {
		if(Vehicle != null) {
			return Vehicle.IsInUse;
		} else {
			return false;
		} 
	} set {} }

	// smooth in-world positions for moving tile to tile
	public Vector2 currentPosition;
	public Vector2 targetPosition;

	// coordinate position on grid for moving destination to destination 
	public Vector2I currentCoordinates;
	public Vector2I targetCoordinates;

	private SimPath currentPath;

	private SimInfraType.DestinationType destinationType;
	
	private GodotObject visualAgent;
	private GodotObject world;
	private int timer = 0, ticksToWait = 1; //Timer for ticks
	private int pointInList;
	private SimGrid simGrid;
	Pathfinding pathfinder;


	//TODO should we instantiate these in a different way? or is visual agent the node instance?
	// Randomize properties for this agent when it first spawns. does it have access to a car? how does it weight different factors?
	public SimAgent(float nonDriverProbability, Vector2I coordinates)
	{
		canDrive = GD.Randf() > nonDriverProbability;
		if(!canDrive) {
			weightFactorsTransportMode[(int)SimVehicleType.TransportMode.CAR] = cantTakeTransitTypeWeightMod;
		}
		currentCoordinates = coordinates;
		//TODO set Vehicle to pedestrian by default, we'll need a list to get this
		destinationType = Sim.Instance.GetTile(currentCoordinates.X, currentCoordinates.Y).DestinationType;
		state = State.AT_DESTINATION;

		//pathFinder = GetNode<SimPath>("../SimPath"); //get a reference to the pathfinder
		//SetRandomTarget();
	}
	
	public void CreateVisualVersion()
	{
		world = GetNode("../../View/World");
		visualAgent = (GodotObject)world.Call("init_agent");
	}

	public override void _Ready()
	{
		base._Ready();
		//DEBUG & TESTING:
		SimPath simPath = new SimPath();
		simPath.vertices = new List<PathVertex>(); 

		simPath.vertices.Add(new PathVertex(new Vector2I(0,0), new Vector2(4,4), null));
		simPath.vertices.Add(new PathVertex(new Vector2I(0,0), new Vector2(7,7), null));
		simPath.vertices.Add(new PathVertex(new Vector2I(0,0), new Vector2(9,9), null));
		simPath.vertices.Add(new PathVertex(new Vector2I(0,0), new Vector2(4,9), null));
		simPath.vertices.Add(new PathVertex(new Vector2I(0,0), new Vector2(7,9), null));
		simPath.vertices.Add(new PathVertex(new Vector2I(0,0), new Vector2(2,9), null));
		simPath.pathVehicleType = (SimVehicleType)ResourceLoader.Load("res://Resources/VehicleTypes/car.tres");
		targetPosition = new Vector2(2,9);

		currentPath = simPath;
		simGrid = GetNode<SimGrid>("..//SimGrid");
	}

	// every game tick. update position smoothly
	public override void _Process(double delta)
	{
		//Vehicle._Process(GetProcessDeltaTime());
	}

	// every simulation tick.
	public void Tick() {
		if(state == State.AT_DESTINATION) {
			GD.Print($"Timer: {timer}");
			if (timer > ticksToWait) 
			{
				GD.Print("Timer Completed!");
				//ChooseNewDestinationType();
				//SetRandomTarget(); //Choose target
				//currentPath = Add pathfinding call here
				//visualAgent.Call("Set_Vehicle", currentPath.pathVehicleType.ModelPath); //change visual model
				timer = 0;
				state = State.TRAVELLING;
				visualAgent.Call("Set_Visible", true);
			} else 
			{
				timer++;
				//visualAgent.Call("Set_Visible", false);
			}

		} else if(state == State.TRAVELLING) {

			if (currentPosition == targetPosition) //might need to compare x/y properties instead
			{
				Arrived();

			} else 
			{
				PathVertex currentStartVertex = currentPath.vertices[pointInList];
				PathVertex currentDestVertex = currentPath.vertices[pointInList + 1];
				if (MoveToNextVertex(currentStartVertex,currentDestVertex))
				{
					pointInList++; //remove vertex already visited, then next vertex will be the start

				}
				Vehicle?.Tick(); //add emissions
			}
		}
	}

	private void Arrived() {

		GD.Print("ARRIVED!");
		state = State.AT_DESTINATION;

		//TODO generate support based on the route 

	}

	//TODO we'll be replacing this with ChooseTarget()
	private void SetRandomTarget()
	{
		targetPosition = new Vector2(GD.RandRange(0, Sim.Instance.grid.Width), GD.RandRange(0, Sim.Instance.grid.Height));
		//Vehicle.SetTarget(targetPosition);
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

	// choose the destination of the chosen destination type to pathfind to 
	// Find the least weighted path to a tile of the destination type. (Mini Metro style.)
	// TODO: if we use a different algorithm, this can: Constitutes both finding a path and determining which tile to travel to next. 
	public void FindNearestDestination(SimInfraType.DestinationType type) {

		//TODO search for the nearest tile with infrastructure with chosen destination type 

		for(int dist = 1; dist < Mathf.Min(Sim.Instance.grid.Width, Sim.Instance.grid.Height); dist++) {
			for(int i = 0; i < dist; i++) {
				//TODO 
			}
		}
		// nothing found 
		//TODO ChooseNewDestinationType different type 

	}

	// Move to the next vertex on the path, returns true if successful
	bool MoveToNextVertex(PathVertex currentVertex, PathVertex nextVertex) {
		if (!nextVertex.TryAddOccupancy(currentPath.pathVehicleType.Mode))
		{
			currentVertex.TryRemoveOccupancy(currentPath.pathVehicleType.Mode); //Remove occupancy from previous
			
			currentPosition = new Vector2(simGrid.GridToWorldPos(true, (int)nextVertex.WorldPosition.X), simGrid.GridToWorldPos(false, (int)nextVertex.WorldPosition.Y)); //Move to next vertex
			visualAgent.Call("Set_Pos", new Vector3(currentPosition.X, 0.22f, currentPosition.Y));
			GD.Print($"Moving from: {currentVertex.WorldPosition} to: {nextVertex.WorldPosition}");
			return true;
		} else
		{
			return false;
		}
		  
	}

	// how much support this agent calculates from this edge 
	public float SupportGainedConnection(PathEdge edge, SimVehicleType.TransportMode mode) {

		float support = 0; 

		support += edge.Safety * suppFactorSafety;
		support += edge.Distance * suppFactorDistance;
		support += SimVehicleType.TypeFromEnum(mode).Emissions * suppFactorEmissions;
		support += suppFactorsTransportMode[(int)mode];

		//TODO consider the max speed of [whatever's smaller: the current vehicle or the infrastructure for that vehicle]
		// really: consdier time 

		//TODO we might want support for transit mode to be a one time thing when they first switch to it, and then distance can outweigh, rather than more tiles travelled increasing it
		// and start out a lump sum that distance decreases from 

		if(support < 0) return 0f;

		return support;
	}

	// Calculate how much this agent weights this connection between 2 tiles.
	public float WeightConnection(PathEdge edge, SimVehicleType.TransportMode mode) {

		float cost = 0;

		cost += edge.Safety * weightFactorSafety;
		cost += edge.Distance * weightFactorDistance;
		cost += SimVehicleType.TypeFromEnum(mode).Emissions * weightFactorEmissions;
		cost += weightFactorsTransportMode[(int)mode];

		//TODO consider the max speed of [whatever's smaller: the current vehicle or the infrastructure for that vehicle]
		// really: consider time 

		if(cost < 0) return 0f;

		return cost;
	}
}




