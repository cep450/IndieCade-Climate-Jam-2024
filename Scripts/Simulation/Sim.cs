using Godot;
using System;
using System.Collections.Generic;
using static SimInfra;

public partial class Sim : Node
{
	/*
	 * Controls simulation game state and execution order.
	 */

	// game state 
	public enum GameState {
		TUTORIAL,	// the game has not begun yet
		GAMEPLAY,	// the game is currently running, but the clock/simulation might be paused
		END_LOSS,	// end state
		END_WIN		// end state 
	}

	public static Sim Instance { get; private set; }

	public SimGrid grid;
	public SimEmissionsMeter EmissionsMeter { get; private set; }
	public SimSupportPool SupportPool { get; private set; }
	private List<SimAgent> agents;
	public SimClock Clock { get; private set; }

	public GameState gameState = GameState.GAMEPLAY;


	//TODO put this in level info once this is merged 
	float nonDriverProbability = 0.3f; // does this agent have access to a car? TODO see if we can find this figure-- most immediately accessible statistics only measure adults, or households
	

	// shortcuts 
	//TODO it might make more sense for these to be in SimGrid
	public SimTile GetTile(int x, int y) {
		return Instance.grid.GetTile(x, y);
	}
	public SimInfraType.DestinationType GetDestinationType(int x, int y) {
		return Instance.grid.GetTile(x,y).DestinationType;
	}

	public Godot.Collections.Array GetInfra(int tileX, int tileY) {
		// Convert List<SimInfra> to Godot.Collections.Array
		Godot.Collections.Array array = new Godot.Collections.Array();
		//List<SimInfra> InfraList = Instance.grid.GetTile(tileX, tileY).Infra;
		//for (int i = 0; i < InfraList.Count; i++)
		//{
			//array.Add(InfraList[i]);
		//}
		return array;
	}
	
	//test func
	public void SayHi() { GD.Print("hi"); }

	public override void _Ready()
	{
		Instance = this;
		grid = GetNode<SimGrid>("SimGrid");
		EmissionsMeter = GetNode<SimEmissionsMeter>("SimEmissionsMeter");
		SupportPool = GetNode<SimSupportPool>("SimSupportPool");
		agents = new List<SimAgent>();
		Clock = GetNode<SimClock>("SimClock");

		BeginGame();
	}

	// Start the simulation for the first time. 
	public void BeginGame() {

/*
		for (int i = 0; i < numberAgents; i++)
		{
			SimAgent agent = new SimAgent(0.3f, new Vector2Int(0,0)); //TODO randomize starting position to start in homes
			agents.Add(agent);
			AddChild(agent);
		}
		*/

		gameState = GameState.GAMEPLAY;
		Clock.UnPause();
	}

	// Simulation logic tick. 
	// Enforce execution order. 
	// The clock calls this when the game is running. 
	public void SimulationTick() {

		foreach (var agent in agents)
		{
			agent.Tick();
		}

		EmissionsMeter.UpdateEmissions(agents);

		EmissionsMeter.EndTick();

	}

	// Visual tick. (Separate from simulation logic tick.) 
	public void VisualTick() {
		//TODO we should talk about if we want to implement this, how, and for what purposes exactly 
	}



	/*
		public void MakeInfraChange(Vector2 tilePosition, SimInfra.InfraType newInfra)
		{
			//Identify the Target Tile: Determine which tile or set of tiles will be affected by the infrastructure change.
			//Determine the New Infrastructure: Identify what type of infrastructure will be added or modified (e.g., road, bike lane).
			//Update the Grid and Edges: Modify the SimGrid and associated SimEdge instances to reflect the new infrastructure.
			//Update Agents: Recalculate paths for agents if necessary.


			SimTile tile = grid.GetTile(tilePosition);
			if (tile == null)
			{
				GD.Print("Tile does not exist at position: " + tilePosition);
				return;
			}

			// Create new infrastructure
			SimInfra newInfra = new SimInfra(newInfra, GetBaseWeightForInfra(newInfra));

			// Set the new infrastructure on the tile
			tile.SetInfra(newInfra);

			// Update the edges connected to this tile
			foreach (SimEdge edge in tile.Edges)
			{
				edge.BaseWeight = newInfra.GetBaseWeight();
				foreach (var mode in edge.SupportedModes)
				{
					edge.Weights[mode] = edge.CalculateWeight(mode);
				}
			}

			// Optionally: Recalculate paths for all agents
			foreach (var agent in agents)
			{
				// Assuming there is a method in SimAgent to recalculate path
				agent.RecalculatePath();
			}

			GD.Print("Infrastrcture updated at position: " + tilePosition);
		}

		private float GetBaseWeightForInfra(SimInfra.InfraType infraType)
		{
			// Define base weights for different types of infra
			switch (infraType)
			{
				case SimInfra.InfraType.Road:
					return 10.0f;
				case SimInfra.InfraType.BikeLane:
					return 5.0f;
				case SimInfra.InfraType.Sidewalk:
					return 3.0f;
				case SimInfra.InfraType.BusLane:
					return 7.0f;
				case SimInfra.InfraType.Rail:
					return 2.0f;
				default:
					return 10.0f;
			}
		}
		*/



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
			SimAgent agent = new SimAgent(nonDriverProbability, position); //TODO get chance to not have a car from level data 
			agents.Add(agent);
			AddChild(agent);
		}
	}

	//TODO right now for simplicity this just removes arbitrary agents since they're considered identical, but in the future, we could pick out specific ones to remove like having a Home save the agents attached to it and remove those specific agents if removed
	public void RemoveAgents(int number) {
		agents.RemoveRange(agents.Count - number + 1, agents.Count - 1);
	}
}
