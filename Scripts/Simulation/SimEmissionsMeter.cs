using Godot;
using System.Collections.Generic;

public partial class SimEmissionsMeter : Node
{
	/* 
	 *	Keeps track of global values.
	 *  Singleton.
	 *  for use by vehicles and anything we want to count towards emissions 
	 */

	private float emissions = 0; // running total
	private const float emissionsCap = 10000; // if emissions surpass this the game ends with a loss. TODO tune this 
	private const float emissionsTarget = 10; // if emission rate goes below this the game ends with a win. TODO tune this

	private float emissionsThisTick = 0;
	private float emissionsLastTick = 0;

	// for use by UI
	public float EmissionRate { get => emissionsLastTick; private set {} }
	public float Emissions { get => emissions; private set {} }

	public float GetEmissions() { return emissions; }
	public float GetEmissionsCap() { return emissionsCap; }

	//Singleton instance
	private static SimEmissionsMeter instance = null;
	public static SimEmissionsMeter Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new SimEmissionsMeter();
			}
			return instance;
		}
	}
	private SimEmissionsMeter() { }


	public void EndTick() {
		emissions += emissionsThisTick;
		CheckEmissionsLevel();
		emissionsLastTick = emissionsThisTick;
		emissionsThisTick = 0;
	}

	public void AddEmissions(float amount)
	{
		emissionsThisTick += amount;
	}

	public void ReduceEmissions(float amount)
	{
		emissionsThisTick -= amount;
	}

	
	private void CheckEmissionsLevel()
	{
		//if emissions surpass a threshold, end the game with a loss 
		if (emissions >= emissionsCap && Sim.Instance.gameState == Sim.GameState.GAMEPLAY)
		{
			Sim.Instance.GameOverEmissions();
		}

		//if emissions rate has been lowered below the threshold, end the game with a win
		if(emissionsThisTick < emissionsTarget) {
			Sim.Instance.GameOverSuccess();
		}
	}

	public void UpdateEmissions(List<SimAgent> activeAgents)
	{
		float totalEmissions = 0;

		foreach (var agent in activeAgents)
		{
			if (agent.Vehicle.IsInUse)
			{
				totalEmissions += agent.Vehicle.Emissions;
			}
		}

		AddEmissions(totalEmissions);
	}
}