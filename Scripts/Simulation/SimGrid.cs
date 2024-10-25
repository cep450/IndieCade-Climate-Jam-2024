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

	// int versions 
	public float GridToWorldPos(int sizeAxis, int coord) {
		float pos = coord + 0.5f;
		pos -= (sizeAxis / 2);
		return pos;
	}
	public float GridToWorldPos(bool useWidth, int coord) {
		if (useWidth)
		{
			return GridToWorldPos(width, coord);
		} else {
			return GridToWorldPos(height, coord);
		}
	}

	// float versions 
	public float GridToWorldPos(int sizeAxis, float coord) {
		float pos = coord + 0.5f;
		pos -= (sizeAxis / 2);
		return pos;
	}
	public Vector2 GridToWorldPos(float x, float y) {
		return new Vector2(GridToWorldPos(width, x), GridToWorldPos(height, y));
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
				grid[x,y].AddInfraFromMask(infraMask, true, false, false);
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
		GD.Print("Data Loaded");
		/*
		SaveGridAsResource();*/
		//var startData = GD.Load<StartData>("res://Scripts/Simulation/CustomResources/SavedData.tres");
		//LoadGridFromResource(startData);
	}

	public SimTile GetTile(int x, int y)
	{
		if (x < 0 || y < 0 || x >= width || y >= height)
		{
			//GD.Print("Tried to get a tile on the grid that was out of range at " + x + ", " + y);
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
	
	public void SaveGridAsResource(string saveFileName = "SavedData") 
	{
		var startData = GD.Load<StartData>("res://Resources/Maps/SavedData.tres");
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
		ResourceSaver.Save(startData, "res://Resources/Maps/" + saveFileName + ".tres");
		GD.Print("Data Saved to file " + saveFileName + ".tres");
	}
	
	public PathVersion GetVersion(Vector2I currentTile, SimInfraType targetType)
	{
		double y_rot = 0.0;
		string versionPath = "Straight";
		List<SimTile> neighbors = new List<SimTile>();
		//get orthoganal neighbors
		Vector2I[] shifts = { new Vector2I(0,1), new Vector2I(0, -1), new Vector2I(-1,0), new Vector2I(1,0) };
		foreach (Vector2I shift in shifts)
		{
			Vector2I cordChecked = currentTile + shift;
			// Catch out of bounds stuff
			if (cordChecked.X == -1 || cordChecked.Y == -1 || cordChecked.X > Width - 1 || cordChecked.Y > Height - 1)
				continue;
			
			SimTile tile = grid[cordChecked.X,cordChecked.Y];
			foreach (SimInfra infra in tile.Infra)
			{
				if (infra.Type == targetType)
				{
					neighbors.Add(tile);
				}
			}
		}
		bool TileUp = false; bool TileDown = false; bool TileLeft = false; bool TileRight = false;
		SimTile tileToCheck = grid[currentTile.X,currentTile.Y];
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
			}
			switch (tile.Coordinates.Y - currentTile.Y)
			{
				case (1):
					TileDown = true;
					break;
				case (-1):
					TileUp = true;
					break;
			}
		}
		switch (neighbors.Count)
		{
			case (1):
				versionPath = "Straight";
				if (TileLeft || TileRight)
					y_rot = Math.PI / 2;
				break;
			case (2):
				if(TileUp && TileDown || TileLeft && TileRight) 
				{
					versionPath = "Straight";
					if (TileLeft)
						y_rot = Math.PI / 2;
					break;
				} 
				else
				{
					versionPath = "Curve";
					if (TileLeft && TileDown)
						y_rot = -(Math.PI / 2);
					if (TileUp && TileLeft)
						y_rot = Math.PI;
					if (TileUp && TileRight)
						y_rot = Math.PI / 2;
					break;
				}
			case (3):
				versionPath = "Tjunction";
				if (TileLeft && TileRight && TileDown)
					y_rot = -(Math.PI / 2);
				if (TileUp && TileDown && TileLeft)
					y_rot = Math.PI;
				if (TileLeft && TileRight && TileUp)
					y_rot =  (Math.PI / 2);
				break;
			case (4): 
				versionPath = "Intersection";
				break;
		}
		Vector3 rotationReturn = new Vector3(0.0f,(float)y_rot,0.0f);
		PathVersion returnData = new PathVersion(versionPath,rotationReturn);
		//Debug printing GD.Print($"Tile: {tileToCheck.Coordinates.X}, {tileToCheck.Coordinates.Y} Neighbor Count: {neighbors.Count} TileUp: {TileUp} TileDown: {TileDown} TileLeft: {TileLeft} TileRight: {TileRight} y_rot: {y_rot}");
		return returnData;
	}
}
