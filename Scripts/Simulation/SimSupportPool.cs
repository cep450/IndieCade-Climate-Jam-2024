using Godot;
using System;
using System.Runtime.InteropServices;

public partial class SimSupportPool : Node
{
	/* 
	 *	Keeps track of global values.
	 *  Singleton.
	 */
	
	public int Support { get; private set;}      // authoritative 

	public void Init(StartData startData) {
		Support = startData.initialSupport;
	}

	// returns true if the player has enough support to be able to spnd this amount of support-- can use for validation

	public bool HaveEnoughSupport(int amount) {
		return (amount <= Support) ? true : false;
	}

	// subtract this amount of support 
	public void SpendSupport(int amount) {
		if (HaveEnoughSupport(amount)) {
			Support -= amount;
			//GD.Print("spent " + amount + " support");
		} else {
			//GD.Print("Not Enough Support!"); //replace in future with UI
		}
	}
	//if we ever want to set/get the raw value
	public void SetSupport(int amount) {
		Support = amount;
	}

	public void AddSupport(float amount) {
		int iamount = (int)amount;
		Support += iamount;
	}

}
