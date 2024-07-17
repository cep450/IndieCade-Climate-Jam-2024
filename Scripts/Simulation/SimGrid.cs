using Godot;
using System;
using System.Collections.Generic;

public partial class SimGrid : Node
{

	/* 
	 * A 2-dimensional grid of tiles.
	 */
	
	public static readonly float TILE_WORLD_SCALE = 1f; //the size of the tile model in the world 
	
	private int width = 10; // Set your grid width
	private int height = 10; // Set your grid height

	public int Width { get => width; private set {}}
	public int Height { get => height; private set {}}

	public SimTile[][] grid;
	
	//TODO for choosing destinations maybe we do all the pathfinding during that choice, where we aren't pathfinding to a particular tile but instead pathfinding until we find a particular type
	//public SimInfraType.DestinationType destinationGrid; // parallel grid just storing destination types for pathfinding 

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public void InitializeGrid()
	{
		grid = new SimTile[width][];
		for (int x = 0; x < width; x++)
		{
			grid[x] = new SimTile[height];
			for (int y = 0; y < height; y++)
			{				
				Vector2I coordinates = new Vector2I(x, y);
				Vector2 position = new Vector2(x * TILE_WORLD_SCALE, y * TILE_WORLD_SCALE);
				grid[x][y] = new SimTile(coordinates, position); // Initialize each tile with a position
				AddChild(grid[x][y]); // Add each tile as a child node (optional)

				//TODO add infrastructure to this tile based on the level file.
				
				// For testing, make each tile a road.
				//SimInfra infra = new SimInfrastructure(, 1.0f);
				//grid[x][y].SetInfrastructure(infra);
			}
		}
	}

	public SimTile GetTile(int x, int y)
	{
		if (x < 0 || y < 0 || x >= grid.Length || y >= grid[0].Length)
		{
			GD.Print("Tried to get a tile on the grid that was out of range.");
			return null;
		}
		return grid[x][y];
	}

//TODO it's probably more efficient to calculate connections when infrastructure is added/removed and save the connections.
	public List<SimTile> GetNeighboursOfType(SimTile tileToCheck, SimInfraType.InfraType type)
	{
		List<SimTile> neighbours = new List<SimTile>();

		//search in 3x3 around the tile to check
		for (int x = -1; x <= 1; x++)
		{
			for (int y = -1; y <= 1; y++)
			{
				if (x == 0 && y == 0) //skip current tile
					continue;
				
				int checkX = tileToCheck.Coordinates.X + x;
				int checkY = tileToCheck.Coordinates.Y + y;

				if (checkX >= 0 && checkX < width && checkY >= 0 && checkY < height)
				{
					if (grid[checkX][checkY].Infra[0].Type.type == type)
					{
						neighbours.Add(grid[checkX][checkY]);
					}
				}

			}
		}
		return neighbours;
	}
}
