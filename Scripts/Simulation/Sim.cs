using Godot;
using System;
using System.Collections.Generic;
using static SimInfrastructure;

public partial class Sim : Node
{
	/*
	 * 
	 */

	// used to determine what can use what types of connections 
	public enum TransitType
	{
		PEDESTRIAN = 0,
		BICYCLE = 1,
		CAR = 2
	}

	private SimGrid grid;
	private SimEmissionsMeter emissionsMeter;
	private SimSupportPool supportPool;
	private List<SimAgent> agents;
	private SimClock clock;

	public override void _Ready()
	{
		grid = GetNode<SimGrid>("SimGrid");
		emissionsMeter = GetNode<SimEmissionsMeter>("SimEmissionsMeter");
		supportPool = GetNode<SimSupportPool>("SimSupportPool");
		agents = new List<SimAgent>();
		clock = GetNode<SimClock>("SimClock");

		for (int i = 0; i < 10; i++)
		{
			var vehicleType = new SimVehicleType(SimVehicleType.TransportMode.Car, 1.0f, 5.0f, new HashSet<SimEdge.TransportMode> { SimEdge.TransportMode.Road });
			var vehicle = new SimVehicle(vehicleType, new Vector2(0, 0));
			SimAgent agent = new SimAgent(vehicle);
			agents.Add(agent);
			AddChild(agent);
		}


	}

	public override void _Process(double delta)
	{
		clock.UpdateTime((float)delta);
		foreach (var agent in agents)
		{
			agent.UpdateAgent();
		}

		emissionsMeter.UpdateEmissions(agents, (float)delta);

	}
	/*
		public void MakeInfrastructureChange(Vector2 tilePosition, SimInfrastructure.InfrastructureType newInfrastructure)
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
			SimInfrastructure newInfra = new SimInfrastructure(newInfrastructure, GetBaseWeightForInfrastructure(newInfrastructure));

			// Set the new infrastructure on the tile
			tile.SetInfrastructure(newInfra);

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

			GD.Print("Infrastructure updated at position: " + tilePosition);
		}

		private float GetBaseWeightForInfrastructure(SimInfrastructure.InfrastructureType infrastructureType)
		{
			// Define base weights for different types of infrastructure
			switch (infrastructureType)
			{
				case SimInfrastructure.InfrastructureType.Road:
					return 10.0f;
				case SimInfrastructure.InfrastructureType.BikeLane:
					return 5.0f;
				case SimInfrastructure.InfrastructureType.Sidewalk:
					return 3.0f;
				case SimInfrastructure.InfrastructureType.BusLane:
					return 7.0f;
				case SimInfrastructure.InfrastructureType.Rail:
					return 2.0f;
				default:
					return 10.0f;
			}
		}
		*/
}
