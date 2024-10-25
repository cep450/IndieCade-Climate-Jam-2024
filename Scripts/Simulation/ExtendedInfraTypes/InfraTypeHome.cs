using Godot;
using System;

[GlobalClass]
public partial class InfraTypeHome : SimInfraType {

	// This infrastructure adds Agents when it is added to the map.

	[Export] public int minAgentsToAdd, maxAgentsToAdd;

	public int AgentsLivingHere { get; private set; }

	public override void AddedToTile(SimTile tile)
	{
		base.AddedToTile(tile);

		AgentsLivingHere = GD.RandRange(minAgentsToAdd, maxAgentsToAdd);

		SimAgent[] agents = Sim.Instance.AddAgents(AgentsLivingHere, tile.Coordinates);

		foreach(SimAgent agent in agents) {
			agent.SetHome(tile);
		}
	}

	public override void RemovedFromTile(SimTile tile)
	{
		base.RemovedFromTile(tile);

		foreach(SimAgent agent in tile.Agents) {
			agent.SetHome(null);
			//TODO if we want agents moving around homes, housing supply to be a mechanic we can implement this here or in agent.SetHome- instead of RemoveAgent
			Sim.Instance.RemoveAgent(agent);
		}

	}
}
