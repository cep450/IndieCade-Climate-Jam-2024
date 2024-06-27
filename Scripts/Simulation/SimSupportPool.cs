using Godot;
using System;

public partial class SimSupportPool : Node
{
	/* 
	 *	Keeps track of global values.
	 *  Singleton.
	 */
	
	int support;            // authoritative 

	float globalHappiness;  // this one isn't authoritative, only a convenient value to get for the ui-- each agent knows their own happiness and updates it accordingly
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}


	// whenever we want this to happen, 
	// calculate global happiness 
	// add to support based on global happiness


	// returns true if the player has enough support to be able to spnd this amount of support-- can use for validation
	public bool HaveEnoughSupport(int amount) {
		return (amount <= support) ? true : false;

	}

	// subtract this amount of support 
	public void SpendSupport(int amount) {
		if (HaveEnoughSupport(amount)) {
			support -= amount;
		} else {
			GD.Print("Not Enough Support!"); //replace in future with UI
		}
	}
	//if we ever want to set/get the raw value
	public void SetSupport(int amount) {
		support = amount;
	}
	public int GetSupport() {
		return support;
	}

}
