using Godot;
using System;
using System.Collections.Generic;
using static SimInfra;

public partial class Sim : Node
{
	/*
	 * Controls simulation game state and execution order.
	 */

	bool DEBUG = false;

	// game state 
	public enum GameState {
		TUTORIAL,	// the game has not begun yet
		GAMEPLAY,	// the game is currently running, but the clock/simulation might be paused
		END_LOSS,	// end state
		END_WIN		// end state 
	}

	public static Sim Instance { get; private set; }

	[Export] public StartData startData;

	public SimGrid grid;
	public PathfindingGraph PathGraph;
	public SimEmissionsMeter EmissionsMeter { get; private set; }
	public SimSupportPool SupportPool { get; private set; }
	private List<SimAgent> agents;
	public SimClock Clock { get; private set; }

	public GameState gameState = GameState.TUTORIAL;

	GDScript mainScript = GD.Load<GDScript>("res://Scripts/main.gd");
	GodotObject mainObject;

	// shortcuts 
	//TODO it might make more sense for these to be in SimGrid
	public SimTile GetTile(int x, int y) {
		return Instance.grid.GetTile(x, y);
	}
	
	public SimInfraType.DestinationType GetDestinationType(int x, int y) {
		return Instance.grid.GetTile(x,y).DestinationType;
	}

	public Godot.Collections.Array<SimInfraType> GetInfra(int tileX, int tileY) {
		SimInfraType.InfraType mask = GetTile(tileX, tileY).InfraTypesMask;
		return SimInfraType.TypesFromEnum(mask);
	}
	
	//test func
	public void SayHi() { GD.Print("hi"); }

	public override void _Ready()
	{
		Instance = this;
		Instance.startData = (StartData)ResourceLoader.Load("res://Scripts/Simulation/CustomResources/SavedData.tres");
		//Give Global access to this node
		GodotObject autoload = GetNode("/root/Global");
		autoload.Call("set_sim",Instance);
		grid = GetNode<SimGrid>("SimGrid");
		EmissionsMeter = GetNode<SimEmissionsMeter>("SimEmissionsMeter");
		SupportPool = GetNode<SimSupportPool>("SimSupportPool");
		agents = new List<SimAgent>();
		Clock = GetNode<SimClock>("SimClock");
		mainObject = GetNode("../../Main");
		LoadMap();
		//TODO we might want a button to call this function instead.
		BeginGame();
	}

	// load level data from save
	public void LoadMap() { /*TODO maybe have this take in a startData resource, 
		but for now, it's just the one given to the sim instance*/
		EmissionsMeter.InitializeEmissionsInfo(startData);
		SupportPool.Init(startData);
		Clock.InitializeClockInfo(startData);

		//generate a grid based on map data 
		grid.LoadGridFromResource(startData);
		PathGraph = new PathfindingGraph(startData.GridWidth, startData.GridHeight);

		foreach(SimAgent agent in agents) {
			agent.InitAfterMapLoad();
		}

	}

	// Start the simulation for the first time. 
	public void BeginGame() {

		gameState = GameState.GAMEPLAY;
		Clock.UnPause();
	}

	// Simulation logic tick. 
	// Enforce execution order. 
	// The clock calls this when the game is running. 
	public void SimulationTick() {
		if(DEBUG) GD.Print("Sim Tick!");
		foreach (var agent in agents)
		{
			agent.Tick();
		}

		EmissionsMeter.UpdateEmissions(agents);

		EmissionsMeter.EndTick();

		mainObject.Call("tick");

	}

	// Visual tick. (Separate from simulation logic tick.) 
	public void VisualTick() {
		//TODO we should talk about if we want to implement this, how, and for what purposes exactly 
	}


		// endings 
		public void GameOverEmissions() {
			GameOver();
			GD.Print("Game Over: Emissions cap reached!");
			gameState = GameState.END_LOSS;
		}

		public void GameOverSupport() {
			GameOver();
			GD.Print("Game Over: Support lost, you were removed from office!");
			gameState = GameState.END_LOSS;
		}

		// 
		public void GameOverTime() {
			GameOver();
			GD.Print("Game Over: The target year to reduce emissions by has passed! You broke your campaign promise and you were removed from office!");
			gameState = GameState.END_LOSS;
		}

		public void GameOverSuccess() {
			GameOver();
			GD.Print("Game Over: You Win!");
			gameState = GameState.END_WIN;
		}

		// things that need to happen in all game overs 
		private void GameOver() {
			Clock.Pause();
		}



	//TODO we probably want agent stuff in its own script like an AgentManger-- we can refactor this after the jam since we're tight on time 

	public void AddAgents(int number, Vector2I position) {

		for(int i = 0; i < number; i++) {
			SimAgent agent = new SimAgent(startData.nonDriverProbability, position); //TODO get chance to not have a car from level data 
			agents.Add(agent);
			AddChild(agent);
			agent.CreateVisualVersion();
		}
	}

	//TODO right now for simplicity this just removes arbitrary agents since they're considered identical, but in the future, we could pick out specific ones to remove like having a Home save the agents attached to it and remove those specific agents if removed
	//TODO does not work right now
	public void RemoveAgents(int number) {
		//agents.RemoveRange(agents.Count - number + 1, agents.Count - 1);
	}
}
