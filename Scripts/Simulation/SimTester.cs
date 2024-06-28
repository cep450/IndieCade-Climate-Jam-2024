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
	SimSupportPool supportPool = new SimSupportPool();
	//List<SimAgent> agents;
	
	[TestCase]
	public void AddEmissions() 
	{

	}
	
	[TestCase]
		public void SpendSupport() 
		{
		}

}
