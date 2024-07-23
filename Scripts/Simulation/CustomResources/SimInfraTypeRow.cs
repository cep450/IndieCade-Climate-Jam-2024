using Godot;
using System;

[GlobalClass]
public partial class SimInfraTypeRow : Resource
{
	[Export] public Godot.Collections.Array<SimInfraType.InfraType> gridData;
}
