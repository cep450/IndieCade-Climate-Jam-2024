using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;


public partial class SimAgent : Node
{

	bool DEBUG = false;

	float agentVerticalPos = 0.4f;

		// TODO generate these properly later
	float suppFactorSafety = 1f;
	float suppFactorDistance = -0.1f;
	float suppFactorEmissions = -0.1f;
									// pedestrian, car, bike
	float [] suppFactorsTransportMode = { 0.1f, 0f, 0.05f }; //TODO define this properly later
	public float [] suppLumpSumTransportMode = { 1f, 0f, 1f }; //TODO define this properly later

	
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

	// coordinate position on VERTEX GRAPH for moving destination to destination 
	public Vector2I currentTileCoords;
	public Vector2I currentVertexCoords;
	public Vector2I targetCoordinates;

	private SimPath currentPath;

	private SimInfraType.DestinationType destinationType;
	
	private GodotObject visualAgent;
	private GodotObject world;
	private int timer = 0, ticksToWait = 1; //Timer for ticks
	private int pointInList = 0;
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

		// tile coords to vertex coords 
		currentTileCoords = coordinates;
		currentVertexCoords = new Vector2I(PathfindingGraph.TileToVertexCoord(coordinates.X), PathfindingGraph.TileToVertexCoord(coordinates.Y));
		GD.Print(" tile " + currentTileCoords.ToString() + " vert " + currentVertexCoords.ToString());

		state = State.AT_DESTINATION;

		pathfinder = new Pathfinding();
	}

	public void InitAfterMapLoad() {
		//GD.Print(" sim " + (Sim.Instance == null) + " get tile " + (Sim.Instance.GetTile(currentTileCoords.X, currentTileCoords.Y) == null));
		destinationType = Sim.Instance.GetTile(currentTileCoords.X, currentTileCoords.Y).DestinationType;
		pathfinder.Init();
		CreateVisualVersion();
	}
	
	public void CreateVisualVersion()
	{
		GD.Print("visual version called");
		world = GetNode("../../View/World");
		visualAgent = (GodotObject)world.Call("init_agent");
	}

	public override void _Ready()
	{
		base._Ready();
		simGrid = GetNode<SimGrid>("..//SimGrid");
		if(Sim.Instance.gameState == Sim.GameState.GAMEPLAY) {
			InitAfterMapLoad();
		} // otherwise we hold off until the map is fully loaded, if the game has not begun yet and we're loading the map. this is for agents spawned during gameplay e.g. when a house is placed 
	}

	// every game tick. update position smoothly
	public override void _Process(double delta)
	{
		//Vehicle._Process(GetProcessDeltaTime());
	}

	// every simulation tick.
	public void Tick() {

		if(state == State.AT_DESTINATION) {
			//GD.Print($"Timer: {timer}");
			if (timer > ticksToWait) 
			{
				GD.Print("Timer Completed!");
				PathVertex currentV = Sim.Instance.PathGraph.GetVertex(currentVertexCoords);
				ChooseNewDestinationType();
				GD.Print("dest type " + destinationType.ToString() + " currentv " + currentV.PathGraphCoordinates.ToString());
				currentPath = pathfinder.FindPath(currentV, destinationType, this); 
				if(currentPath == null) {
					//if no path that way, 
					//TODO go somewhere else (when we have more than 1 destination type)
					//for now just wait 10 ticks 
					timer = 0;
					return;
				}
				Vehicle = new SimVehicle(currentPath.pathVehicleType); //TODO CHANGE THIS THIS IS VERY BAD
				Vehicle.IsInUse = true;
				//visualAgent.Call("Set_Vehicle", "res://Models/Car/Car_Yellow_Driving.tscn"); //change visual model
				visualAgent.Call("Set_Vehicle", currentPath.pathVehicleType.Index);
				timer = 0;
				state = State.TRAVELLING;
				visualAgent.Call("Set_Visible", true);
				pointInList = 0;
			} else 
			{
				timer++;
				//visualAgent.Call("Set_Visible", false);
			}

		} else if(state == State.TRAVELLING) {

			//TODO factor in travel time, but for now, just 1 tick per tile 

			if (pointInList >= currentPath.vertices.Count - 1)
			{
				Arrived();
			} else {
				PathVertex currentStartVertex = currentPath.vertices[pointInList];
				PathVertex currentDestVertex = currentPath.vertices[pointInList + 1];
				if (MoveToNextVertex(currentStartVertex,currentDestVertex))
				{
					pointInList++; //remove vertex already visited, then next vertex will be the start
				}
				Vehicle?.Tick(); //add emissions

				//TODO do this if successful 
				currentVertexCoords = currentStartVertex.PathGraphCoordinates;
				currentTileCoords = new Vector2I((int)PathfindingGraph.VertexToTileCoord(currentStartVertex.PathGraphCoordinates.X), (int)PathfindingGraph.VertexToTileCoord(currentStartVertex.PathGraphCoordinates.Y));
			}
		}
	}

	private void Arrived() {

		GD.Print("ARRIVED!");
		state = State.AT_DESTINATION;
		Vehicle.IsInUse = false;

		PathVertex lastVert = null;
		if(currentPath.vertices.Count > 1) {
			lastVert = currentPath.vertices[currentPath.vertices.Count - 1];
		} else if(currentPath.vertices.Count > 0 ){
			lastVert = currentPath.vertices[0];
		}
		
		if(lastVert != null ){
			currentVertexCoords = lastVert.PathGraphCoordinates;
			currentTileCoords = new Vector2I((int)PathfindingGraph.VertexToTileCoord(lastVert.PathGraphCoordinates.X), (int)PathfindingGraph.VertexToTileCoord(lastVert.PathGraphCoordinates.Y));
		}
		
		//generate support based on the route 
		Sim.Instance.SupportPool.AddSupport(currentPath.totalSupport);

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

	// Move to the next vertex on the path, returns true if successful
	bool MoveToNextVertex(PathVertex currentVertex, PathVertex nextVertex) {

		visualAgent.Call("Set_Pos", new Vector3(nextVertex.WorldPosition.X, agentVerticalPos, nextVertex.WorldPosition.Y));
		GD.Print($"Moving from: {currentVertex.WorldPosition} to: {nextVertex.WorldPosition}");
		return true;

		// not worrying about occupancy for now 
		/*
		if (!nextVertex.TryAddOccupancy(currentPath.pathVehicleType.Mode))
		{
			currentVertex.TryRemoveOccupancy(currentPath.pathVehicleType.Mode); //Remove occupancy from previous
			
			currentPosition = new Vector2(simGrid.GridToWorldPos(true, (int)nextVertex.WorldPosition.X), simGrid.GridToWorldPos(false, (int)nextVertex.WorldPosition.Y)); //Move to next vertex
			visualAgent.Call("Set_Pos", new Vector3(currentPosition.X, 0.22f, currentPosition.Y));
			//GD.Print($"Moving from: {currentVertex.WorldPosition} to: {nextVertex.WorldPosition}");
			return true;
		} else
		{
			return false;
		}*/
		  
	}

	// how much support this agent calculates from this edge 
	public float SupportGainedConnection(PathEdge edge, SimVehicleType.TransportMode mode) {

		float support = 0; 

		support += edge.Safety * suppFactorSafety;
		support += edge.Distance * suppFactorDistance;
		support += SimVehicleType.TypeFromEnum(mode).Emissions * suppFactorEmissions;
		
				//TODO non bullshit way 
		if(mode == SimVehicleType.TransportMode.PEDESTRIAN) {
			support += suppFactorsTransportMode[0];
		} else if(mode == SimVehicleType.TransportMode.CAR) {
			support += suppFactorsTransportMode[1];
		} else if(mode == SimVehicleType.TransportMode.BIKE) {
			support += suppFactorsTransportMode[2];
		}

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

		//TODO non bullshit way 
		if(mode == SimVehicleType.TransportMode.PEDESTRIAN) {
			cost += weightFactorsTransportMode[0];
		} else if(mode == SimVehicleType.TransportMode.CAR) {
			cost += weightFactorsTransportMode[1];
		} else if(mode == SimVehicleType.TransportMode.BIKE) {
			cost += weightFactorsTransportMode[2];
		}
		

		//TODO consider the max speed of [whatever's smaller: the current vehicle or the infrastructure for that vehicle]
		// really: consider time 

		if(cost < 0) return 0f;

		return cost;
	}
}



