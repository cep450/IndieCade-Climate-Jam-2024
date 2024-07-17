using Godot;
using System;
using System.Collections.Generic;

public partial class SimGrid : Node
{

	/* 
	 * A 2-dimensional grid of tiles.
	 */
	
	public static readonly float TILE_WORLD_SCALE = 1f; //the size of the tile model in the world 
	
	private int width = 10; // will be updated by save resource
	private int height = 10; // will be updated by save resource

	public int Width { get => width; private set {}}
	public int Height { get => height; private set {}}

	public SimTile[,] grid;
	
	//TODO for choosing destinations maybe we do all the pathfinding during that choice, where we aren't pathfinding to a particular tile but instead pathfinding until we find a particular type
	//public SimInfraType.DestinationType destinationGrid; // parallel grid just storing destination types for pathfinding 

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public void LoadGridFromResource(StartData resourceToLoad)
	{
		width = resourceToLoad.GridWidth;
		height = resourceToLoad.GridHeight;

		grid = new SimTile[width,height];

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				Vector2I coordinates = new Vector2I(x, y);
				Vector2 position = new Vector2(x * TILE_WORLD_SCALE, y * TILE_WORLD_SCALE);
				grid[x,y] = new SimTile(coordinates, position); // Initialize each tile with a position
				AddChild(grid[x,y]); // Add each tile as a child node (optional)

				//Tadd infrastructure to this tile based on the mask in the level file.
				SimInfraType.InfraType infraMask = resourceToLoad.gridData[x].gridData[y].type;
				GetTile(x,y).AddInfraFromMask(infraMask, true);
			}
		}

		/*
		SaveGridAsResource();*/
		//var startData = GD.Load<StartData>("res://Scripts/Simulation/CustomResources/SavedData.tres");
		//LoadGridFromResource(startData);
	}

	public SimTile GetTile(int x, int y)
	{
		if (x < 0 || y < 0 || x >= width || y >= height)
		{
			GD.Print("Tried to get a tile on the grid that was out of range.");
			return null;
		}
		return grid[x,y];
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
					if (grid[checkX,checkY].Infra[0].Type.type == type)
					{
						neighbours.Add(grid[checkX,checkY]);
					}
				}

			}
		}
		return neighbours;
	}
	
	public void SaveGridAsResource() 
	{
		var startData = GD.Load<StartData>("res://Scripts/Simulation/CustomResources/StartData.tres");
		startData.gridData = new SimInfraTypeRow[width];
		for (int x = 0; x < width; x++)
		{
			SimInfraTypeRow currentInfraRow = new SimInfraTypeRow();
			currentInfraRow.gridData = new SimInfraType[height];
			for (int y = 0; y < height; y++)
			{
				currentInfraRow.gridData[y] = new SimInfraType();
				currentInfraRow.gridData[y].type = GetTile(x,y).InfraTypesMask;
			}
			startData.gridData[x] = currentInfraRow;
		}
		ResourceSaver.Save(startData, "res://Scripts/Simulation/CustomResources/SavedData.tres");
		
	}

}