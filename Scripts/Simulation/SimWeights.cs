using Godot;
using System;

public class SimWeights {

	

	// associate each index with a name for what it represents 
	public enum Weight {
		TIME = 0,
		SAFETY = 1,
		EMISSIONS = 2
	}

	// example use: 
	// weights[Weight.SAFETY] = 0.5f;

	public float[] weights;

}
