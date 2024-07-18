using Godot;
using System;

[GlobalClass]
public partial class StartData : Resource
{
	[Export] public int emissionsCap = 10000;
	[Export] public int emissionsTarget = 100;
	[Export] public int initialSupport = 10;
	[Export] public int startingYear = 2025;
	[Export] public int yearsUntilGameOver = 15;
	[Export] public float nonDriverProbability = 0.3f;
	
	[Export] public SimInfraTypeRow[] gridData;

	public int GridWidth { get => gridData.Length; private set {} }
	public int GridHeight { get => gridData[0].gridData.Count; private set {} }
}
