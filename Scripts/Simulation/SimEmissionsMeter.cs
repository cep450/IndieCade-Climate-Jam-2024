using Godot;
using System;

public class SimEmissionsMeter
{
	/* 
	 *	Keeps track of global values.
	 *  Singleton.
	 */
	
	int emissions;
	const int emissionsCap = 100; //Placeholder value

	//for use by vehicles and anything we want to count towards emissions 
	public void AddEmissions(int amount) {
		if (amount > 0) {
			emissions += amount;
		}
		if (emissions > emissionsCap) {
			//TODO: add game loss here
		}
		
	}
	
	public int GetEmissions() {
		return emissions;
	}
	
}
