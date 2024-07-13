using Godot;
using System;
using System.Collections.Generic;

public partial class SimGrid : Node
{

	/* 
	 * A 2-dimensional grid of tiles.
	 */

	public SimTile[][] grid;
	private int width = 10; // Set your grid width
	private int height = 10; // Set your grid height

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		InitializeGrid();
	}

	private void InitializeGrid()
	{
		grid = new SimTile[width][];
		for (int x = 0; x < width; x++)
		{
			grid[x] = new SimTile[height];
			for (int y = 0; y < height; y++)
			{				
				Vector2 position = new Vector2(x, y);
				grid[x][y] = new SimTile(position, 1); // Initialize each tile with a position and capacity
				AddChild(grid[x][y]); // Add each tile as a child node (optional)
				
				// For testing, make each tile a road.
				//SimInfra infra = new SimInfrastructure(, 1.0f);
				//grid[x][y].SetInfrastructure(infra);
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public SimTile GetTile(int x, int y)
	{
		if (x < 0 || y < 0 || x >= grid.Length || y >= grid[0].Length)
		{
			GD.Print("ERR: Tried to get a tile on the grid that was out of range.");
			return null;
		}
		return grid[x][y];
	}

	public List<SimTile> GetNeighbours(SimTile tileToCheck, SimInfra.InfraType type)
	{
		List<SimTile> neighbours = new List<SimTile>();

		//search in 3x3 around the tile to check
		for (int x = -1; x <= 1; x++)
		{
			for (int y = -1; y <= 1; y++)
			{
				if (x == 0 && y == 0) //skip current tile
					continue;
				
				int checkX = tileToCheck.gridX + x;
				int checkY = tileToCheck.gridY + y;

				if (checkX >= 0 && checkX < width && checkY >= 0 && checkY < height)
				{
					if (grid[checkX][checkY].Infra[0].Type == type) //TODO (!!!) refactor this to support multiple types of infrastructure on a single tile.
					{
						neighbours.Add(grid[checkX][checkY]);
					}
				}

			}
		}
		return neighbours;
	}
}
