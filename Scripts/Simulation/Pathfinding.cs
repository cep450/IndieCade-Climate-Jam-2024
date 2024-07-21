using Godot;
using System;
using System.Collections.Generic;

public partial class Pathfinding : Node
{

	/*
		Create a SimPath to give to a SimAgent.
	*/
/*
	SimGrid grid;

	public override void _Ready() 
	{
		grid = GetNode<SimGrid>("SimGrid");
	}
*/

	public bool initialized = false;
	float [,] weights;
	bool [,] visited;
	PathVertex [,] predecessors;
	int width, height;

	public void Init() {

		width = Sim.Instance.PathGraph.Width;
		height = Sim.Instance.PathGraph.Height;

		weights = new float[width, height];
		visited = new bool[width, height];
		predecessors = new PathVertex[width, height];

		

		initialized = true;
	}


	//TODO rework this so agents don't use a single type of infrastructure to find a path- let them transfer between them
	SimPath FindPath(PathVertex startVert, SimInfraType.DestinationType destinationType)
	{
		if(!initialized) Init();

		GD.Print("called FindPath");

		// Run Dijkstra's until we land on a vert of the specified destination type. Then, return that path.

		// set up default values 
		for(int x = 0; x < width; x++) {
			for(int y = 0; y < height; y++) {
				weights[x,y] = float.PositiveInfinity;
				visited[x,y] = false;
				predecessors[x,y] = null;
			}
		}

		// set up source 
		weights[startVert.PathGraphCoordinates.X, startVert.PathGraphCoordinates.Y] = 0f;

		
		//TODO replace this when we implement switching, this is dumb 
		//SimPath walk = 
		//SimPath bike = 
		//SimPath car = 
		// TODO just testing one for now 
		Dijkstra(startVert, destinationType, SimVehicleType.TransportMode.CAR);

		//TODO return the smallest weight path 



	}

	//TODO switching between transit modes. but the day is not today. probably 
	//TODO need different arrays to keep track of diff transit modes 
	private void Dijkstra(PathVertex startVert, SimInfraType.DestinationType destinationType, SimVehicleType.TransportMode mode) {

		{


			if()

		}




		List<PathVertex> openSet = new List<PathVertex>();
		HashSet<PathVertex> closedSet = new HashSet<PathVertex>();
		openSet.Add(startVert);

		while (openSet.Count > 0) 
		{
			PathVertex currentVert = openSet[0];
			//set the current tile to the one with lowest fCost and update open/closed sets
			for (int i = 1; i < openSet.Count; i++)
			{
				//if the fCosts are equal, change tile only if the openSet tile has a lower hCost
			   if (openSet[i].fCost() < currentVert.fCost() || openSet[i].fCost() == currentVert.fCost() && openSet[i].hCost < currentVert.hCost) 
			   {
				currentVert = openSet[i];
			   }
			}

			openSet.Remove(currentVert);
			closedSet.Add(currentVert);

			if (currentVert == targetVert) //path has been found
			{
				RetracePath(startVert, targetVert);
				return;
			}

			foreach (SimTile neighbour in grid.GetNeighboursOfType(currentVert, type))
			{
				if (closedSet.Contains(neighbour)) //ignore closed neighbours
				{
					continue;
				}

				int newMovementCostToNeighbour = currentVert.gCost + GetDistance(currentVert, neighbour);
				if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
				{
					neighbour.gCost = newMovementCostToNeighbour;
					neighbour.hCost = GetDistance(neighbour, targetTile);
					neighbour.parent = currentTile;

					if (!openSet.Contains(neighbour))
					{
						openSet.Add(neighbour);
					}
				}
			}
		}
	}

	void RetracePath(SimTile startTile, SimTile endTile)
	{

		List<SimTile> path = new List<SimTile>();
		SimTile currentTile = endTile;

		while (currentTile != startTile)
		{
			path.Add(currentTile);
			currentTile = currentTile.parent;
		}
		path.Reverse();
	}

	int GetDistance(SimTile tileA, SimTile tileB)
	{
		int dstX = Mathf.Abs(tileA.Coordinates.X - tileB.Coordinates.X);
		int dstY = Mathf.Abs(tileA.Coordinates.Y - tileB.Coordinates.Y);

		return dstX + dstY; //since we dont move diagonally
	}

}
