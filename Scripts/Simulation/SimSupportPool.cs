using Godot;
using System;

public partial class SimSupportPool : Node
{
	/* 
	 *	Keeps track of global values.
	 *  Singleton.
	 */

		//TODO we probably want this to be in whatever we're using to store level information, if we want it to be different in different levels.
		const int startingSupport = 100; // how much support you start with on this level
	
	public int Support { get; private set;}      // authoritative 

	public float GlobalHappiness {get; private set;}  // this one isn't authoritative, only a convenient value to get for the ui-- each agent knows their own happiness and updates it accordingly

	public void Init(StartData startData) {
		Support = startData.initialSupport;
	}

	// whenever we want this to happen, 
	// calculate global happiness 
	// add to support based on global happiness


	// returns true if the player has enough support to be able to spnd this amount of support-- can use for validation

	public bool HaveEnoughSupport(int amount) {
		return (amount <= Support) ? true : false;

	}

	// subtract this amount of support 
	public void SpendSupport(int amount) {
		if (HaveEnoughSupport(amount)) {
			Support -= amount;
		} else {
			GD.Print("Not Enough Support!"); //replace in future with UI
		}
	}
	//if we ever want to set/get the raw value
	public void SetSupport(int amount) {
		Support = amount;
	}

}
