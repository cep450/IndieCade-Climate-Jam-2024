namespace Examples;
using Godot;
using System;
using GdUnit4;
using static GdUnit4.Assertions;

[TestSuite]
public class SimTester {


	// used to determine what can use what types of connections 
	public enum TransitType {
		PEDESTRIAN = 0,
		BICYCLE = 1,
		CAR = 2
	}

	//SimGrid grid;
	SimEmissionsMeter emissionsMeter = new SimEmissionsMeter();
	//SimSupportPool supportPool;
	//List<SimAgent> agents;
	
	[TestCase]
	public void AddEmissions() {
		emissionsMeter.AddEmissions(60);
		AssertThat(emissionsMeter.GetEmissions()).IsEqual(60);
	}

}
