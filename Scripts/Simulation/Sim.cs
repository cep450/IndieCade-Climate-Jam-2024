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

	[Export] bool EditorMode = false;

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

	GodotObject godotGlobal;

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

		//TODO throw up a loading screen here 

		GD.Print("sim ready");
		Instance = this;
		if(startData == null) {
			Instance.startData = (StartData)ResourceLoader.Load("res://Scripts/Simulation/CustomResources/SavedData.tres");
		}
		//Give Global access to this node
		godotGlobal = GetNode("/root/Global");
		godotGlobal.Call("set_sim",Instance);
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

		//loadingscreen.SetText("Loading map...");

		//godotGlobal.Set("inDevMode", startData.EditorMode);
		godotGlobal.Set("inDevMode", EditorMode);

		EmissionsMeter.InitializeEmissionsInfo(startData);
		SupportPool.Init(startData);
		Clock.InitializeClockInfo(startData);

		//generate a grid based on map data 
		grid.LoadGridFromResource(startData);
		PathGraph = new PathfindingGraph(startData.GridWidth, startData.GridHeight);

	}

	// Start the simulation for the first time. 
	public void BeginGame() {

		//loadingscreen.SetText("Loading citizens...");

		foreach(SimAgent agent in agents) {
			agent.InitAfterMapLoad();
		}

		gameState = GameState.GAMEPLAY;
		Clock.UnPause();

		//TODO close the loading screen here 
	}

	// Simulation logic tick. 
	// Enforce execution order. 
	// The clock calls this when the game is running. 
	public void SimulationTick() {

		//GD.Print("sim tick");

		foreach (SimAgent agent in agents)
		{
			//GD.Print("agent tick");
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

	public SimAgent[] AddAgents(int number, Vector2I position) {

		SimAgent [] newAgents = new SimAgent[number];
		for(int i = 0; i < number; i++) {
			SimAgent agent = new SimAgent(startData.nonDriverProbability, position); //TODO get chance to not have a car from level data 
			agents.Add(agent);
			AddChild(agent);
			newAgents[i] = agent;
		}
		return newAgents;
	}

	//TODO see if we can optimize this 
	public void RemoveAgent(SimAgent agent) {
		agents.Remove(agent);
		//TODO we should be able to delete visual agents here to free up memory from the loaded models 
	}
}
