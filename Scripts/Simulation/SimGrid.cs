using Godot;
using System;

public partial class SimGrid : Node
{

	/* 
	 * A 2-dimensional grid of tiles.
	 */
	
	public SimTile [][] grid;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public SimTile GetTile(int x, int y) {
		if(x <= 0 || y <= 0 || x > grid.Length || y > grid[0].Length) {
			GD.Print("ERR: Tried to get a tile on the grid that was out of range.");
			return null;
		}
		return grid[x][y];
	}
}
