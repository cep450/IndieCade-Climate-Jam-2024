using Godot;
using System;

[GlobalClass]
public partial class PathVersion : Resource
{
	[Export] public string versionString { get; set; }
	[Export] public Vector3 rotation { get; set; }

	public PathVersion(string v, Vector3 r)
	{
		versionString = v;
		rotation = r;
	}

	public PathVersion()
	{
	}
}
