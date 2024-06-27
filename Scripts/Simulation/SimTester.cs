namespace Examples;
using Godot;
using System;
using GdUnit4;
using static GdUnit4.Assertions;

//Simple testing class for script functions
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
	SimSupportPool supportPool = new SimSupportPool();
	//List<SimAgent> agents;
	
	[TestCase]
	public void AddEmissions() {
		emissionsMeter.AddEmissions(60);
		AssertThat(emissionsMeter.GetEmissions()).IsEqual(60);
	}
	
	[TestCase]
		public void SpendSupport() {
			supportPool.SetSupport(50);
			supportPool.SpendSupport(45);
			AssertThat(supportPool.GetSupport()).IsEqual(5);
			
			AssertThat(supportPool.HaveEnoughSupport(10)).IsEqual(false);
			supportPool.SpendSupport(10);
			AssertThat(supportPool.GetSupport()).IsEqual(5);
		}

}
