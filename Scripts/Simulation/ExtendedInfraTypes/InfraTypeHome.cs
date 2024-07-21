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

		Sim.Instance.AddAgents(AgentsLivingHere, tile.Coordinates);
	}

	public override void RemovedFromTile(SimTile tile)
	{
		base.RemovedFromTile(tile);

		Sim.Instance.RemoveAgents(AgentsLivingHere);
	}
}
