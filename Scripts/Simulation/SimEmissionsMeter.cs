using Godot;
using System.Collections.Generic;

public partial class SimEmissionsMeter : Node
{
	/* 
	 *	Keeps track of global values.
	 *  Singleton.
	 *  for use by vehicles and anything we want to count towards emissions 
	 */

	bool DEBUG = true;

	private float emissions = 0; // running total
	private float emissionsCap = 10000; // if emissions surpass this the game ends with a loss. TODO tune this 
	private float emissionsTarget = 10; // if emission rate goes below this the game ends with a win. TODO tune this

	private float emissionsThisTick = 0;
	private float emissionsLastTick = 0;

	private int winCheckCounter = 0;
	private int winCheckTicks = 10;
	private float emissionsThisCheck = 0;

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

	public void InitializeEmissionsInfo(StartData startData) {
		emissionsCap = startData.emissionsCap;
		emissionsTarget = startData.emissionsTarget;
		emissions = 0;
		emissionsThisTick = 0;
		emissionsLastTick = 0;
	}

	public void EndTick() {
		emissions += emissionsThisTick;
		emissionsThisCheck += emissionsThisTick;
		winCheckCounter++;
		if(winCheckCounter > winCheckTicks) {
			CheckEmissionsLevel(true);
			winCheckCounter = 0;
			emissionsThisCheck = 0;
		} else {
			CheckEmissionsLevel(false);
		}
		emissionsLastTick = emissionsThisTick;
		emissionsThisTick = 0;
		
		GD.Print("emissions this tick: " + emissionsLastTick + " total: " + emissions);
	}

	public void AddEmissions(float amount)
	{
		emissionsThisTick += amount;
	}

	public void ReduceEmissions(float amount)
	{
		emissionsThisTick -= amount;
	}

	
	private void CheckEmissionsLevel(bool checkWin)
	{
		//if emissions surpass a threshold, end the game with a loss 
		if (emissions >= emissionsCap && Sim.Instance.gameState == Sim.GameState.GAMEPLAY)
		{
			Sim.Instance.GameOverEmissions();
		}

		//if emissions rate has been lowered below the threshold, end the game with a win
		if(checkWin && emissionsThisCheck < emissionsTarget) {
			if(!DEBUG) Sim.Instance.GameOverSuccess();
		}
	}

	public void UpdateEmissions(List<SimAgent> activeAgents)
	{
		float totalEmissions = 0;

		foreach (var agent in activeAgents)
		{
			if (agent.VehicleIsInUse)
			{
				totalEmissions += agent.Vehicle.Emissions;
			}
		}

		AddEmissions(totalEmissions);
	}
}