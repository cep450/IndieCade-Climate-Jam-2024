using Godot;
using System.Collections.Generic;

public partial class SimEmissionsMeter : Node
{
	/* 
	 *	Keeps track of global values.
	 *  Singleton.
	 */

	private float emissions;
	private const float emissionsCap = 100000; //example number

	//for use by vehicles and anything we want to count towards emissions 
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

	public float GetEmissions() { return emissions; }
	public float GetEmissionsCap() { return emissionsCap; }

	public void AddEmissions(float amount)
	{

		//if emissions surpass a threshold, end the game with a loss 
		emissions += amount;
		CheckEmissionsLevel();
	}

	public void ReduceEmissions(float amount)
	{
		emissions -= amount;
		if (emissions < 0)
		{
			emissions = 0; // prevent negative emissions
		}
	}

	private void CheckEmissionsLevel()
	{
		if (emissions >= emissionsCap)
		{
			//TODO end the game with a loss
			TriggerGameOver();
		}
	}
	private void TriggerGameOver()
	{
		GD.Print("Game Over: Emissions cap reached!");
		//how does this interact with the time ticking system?
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
