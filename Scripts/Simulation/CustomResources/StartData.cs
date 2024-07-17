using Godot;
using System;

[GlobalClass]
public partial class StartData : Resource
{
	[Export] public int emissionsCap;
	[Export] public int emissionsTarget;
	[Export] public int startingYear;
	[Export] public int yearsUntilGameOver;
 
	[Export] public int GridWidth;
	[Export] public int GridHeight;
	
	[Export] public SimInfraTypeRow[] gridData;
}
