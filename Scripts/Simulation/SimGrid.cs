using Godot;
using System;
using System.Collections.Generic;
using System.Threading;

public partial class SimGrid : Node
{

	/* 
	 * A 2-dimensional grid of tiles.
	 */
	
	public static readonly float TILE_WORLD_SCALE = 1f; //the size of the tile model in the world 
	
	private int width; // will be updated by save resource
	private int height; // will be updated by save resource

	public int Width { get => width; private set {}}
	public int Height { get => height; private set {}}

	public SimTile[,] grid;

	GDScript worldScript = GD.Load<GDScript>("res://Scripts/View/world.gd");
	
	//TODO for choosing destinations maybe we do all the pathfinding during that choice, where we aren't pathfinding to a particular tile but instead pathfinding until we find a particular type
	//public SimInfraType.DestinationType destinationGrid; // parallel grid just storing destination types for pathfinding 

	public float GridToWorldPos(int sizeAxis, int coord) {
		float pos = coord + 0.5f;
		pos -= (sizeAxis / 2);
		return pos;
	}

	public void LoadGridFromResource(StartData resourceToLoad)
	{
		width = resourceToLoad.GridWidth;
		height = resourceToLoad.GridHeight;

		grid = new SimTile[width,height];

		GodotObject world = GetNode("../../View/World");

		// initialize the view world 
		//world.Call("init_world", width, height, TILE_WORLD_SCALE);

		// initialize internal SimGrid with SimTiles 
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				//create a world tile and a SimTile and associate them with each other 
				
				Vector2I coordinates = new Vector2I(x, y);
				Vector2 position = new Vector2(GridToWorldPos(width, x) * TILE_WORLD_SCALE, GridToWorldPos(height, y) * TILE_WORLD_SCALE);

				GodotObject newVisualTile = (GodotObject)world.Call("init_tile", x, y, position.X, position.Y);
				grid[x,y] = new SimTile(coordinates, position, newVisualTile); // Initialize each tile with a position
				AddChild(grid[x,y]); // Add each tile as a child node (optional)

				//Add infrastructure to this tile based on the mask in the level file.
				SimInfraType.InfraType infraMask = resourceToLoad.gridData[x].gridData[y];
				grid[x,y].AddInfraFromMask(infraMask, true);
			}
		}
		// update models 
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				// update visual tile once now that all infra has been added
				grid[x,y].VisualTile.Call("update_visuals");
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
			GD.Print("Tried to get a tile on the grid that was out of range at " + x + ", " + y);
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
			currentInfraRow.gridData = new Godot.Collections.Array<SimInfraType.InfraType>();
			for (int y = 0; y < height; y++)
			{
				currentInfraRow.gridData.Add(GetTile(x,y).InfraTypesMask);
			}
			startData.gridData[x] = currentInfraRow;
		}
		ResourceSaver.Save(startData, "res://Scripts/Simulation/CustomResources/SavedData.tres");
		
	}
	
	public PathVersion GetVersion(Vector2I currentTile, SimInfraType type)
	{
		GD.Print("grid_called");
		float y_rot = 0.0f;
		string versionPath = "Straight";
		List<SimTile> neighbors = new List<SimTile>();

		//get orthoganal neighbors
		//get up
		if (currentTile.Y != 0)
			neighbors.Add(grid[currentTile.X, currentTile.Y - 1]);
		//get down
		if (currentTile.Y < height - 1)
			neighbors.Add(grid[currentTile.X, currentTile.Y + 1]);
		//get left
		if (currentTile.Y != 0)
			neighbors.Add(grid[currentTile.X - 1, currentTile.Y]);
		//get right
		if (currentTile.Y < width -1)
			neighbors.Add(grid[currentTile.X + 1, currentTile.Y]);

		bool TileUp = false; bool TileDown = false; bool TileLeft = false; bool TileRight = false;
		foreach (SimTile tile in neighbors)
		{
			switch (tile.Coordinates.X - currentTile.X)
			{
				case (1):
					TileRight = true;
					break;
				case (-1):
					TileLeft = true;
					break;
				default:
					continue;
			}
			switch (tile.Coordinates.Y - currentTile.Y)
			{
				case (1):
					TileUp = true;
					break;
				case (-1):
					TileUp = true;
					break;
				default:
					continue;
			}
			switch (neighbors.Count)
			{
				case (1):
					versionPath = "Straight";
					if (TileLeft || TileRight)
						y_rot = 90.0f;
					break;
				case (2):
					if(TileUp && TileDown || TileLeft && TileRight) 
					{
						versionPath = "Straight";
						if (TileLeft)
							y_rot = 90.0f;
						break;
					} 
					else
					{
						versionPath = "Curve";
						if (TileLeft && TileDown)
							y_rot = 90.0f;
						if (TileUp && TileLeft)
							y_rot = 90.0f;
						if (TileUp && TileRight)
							y_rot = 90.0f;
						break;
					}
				case (3):
					versionPath = "Tjunction";
					if (TileLeft && TileRight && TileDown)
						y_rot = 90.0f;
					if (TileUp && TileDown && TileLeft)
						y_rot = 180.0f;
					if (TileLeft && TileRight && TileDown)
						y_rot = 270.0f;
					break;
				case (4): 
					versionPath = "Intersection";
					break;
			}
		}
		Vector3 rotationReturn = new Vector3(0.0f,y_rot,0.0f);
		PathVersion returnData = new PathVersion(versionPath,rotationReturn);
		GD.Print("versionPath: " + versionPath);
		GD.Print("versionPathFromReturnData: " + returnData.versionString);
		return returnData;
	}
}
