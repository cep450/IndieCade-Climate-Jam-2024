using Godot;
using System;

public class SimEmissionsMeter
{
	/* 
	 *	Keeps track of global values.
	 *  Singleton.
	 */
	
	int emissions;
	const int emissionsCap = 100;

	//for use by vehicles and anything we want to count towards emissions 
	public void AddEmissions(float amount) {

		//TODO if emissions surpass a threshold, end the game with a loss 
	}

}
