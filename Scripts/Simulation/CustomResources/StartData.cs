using Godot;
using System;

[GlobalClass]
public partial class StartData : Resource
{
	[Export]
	public int emissionsCap;
	
	[Export] 
	public SimInfraTypeRow[] gridData;
}
