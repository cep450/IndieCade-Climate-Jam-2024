using Godot;
using System;
using System.Collections.Generic;

public partial class Pathfinding : Node
{
	SimGrid grid;

	public override void _Ready() 
	{
		grid = GetNode<SimGrid>("SimGrid");
	}

	//TODO rework this so agents don't use a single type of infrastructure to find a path- let them transfer between them
	void FindPath(SimTile startTile, SimTile targetTile)
	{
		List<SimTile> openSet = new List<SimTile>();
		HashSet<SimTile> closedSet = new HashSet<SimTile>();
		openSet.Add(startTile);
    public override void _Ready() 
    {
        grid = GetNode<SimGrid>("SimGrid");
    }
    void FindPath(SimTile startTile, SimTile targetTile, SimInfra.InfraType type)
    {
        List<SimTile> openSet = new List<SimTile>();
        HashSet<SimTile> closedSet = new HashSet<SimTile>();
        openSet.Add(startTile);

		while (openSet.Count > 0) 
		{
			SimTile currentTile = openSet[0];
			//set the current tile to the one with lowest fCost and update open/closed sets
			for (int i = 1; i < openSet.Count; i++)
			{
				//if the fCosts are equal, change tile only if the openSet tile has a lower hCost
			   if (openSet[i].fCost() < currentTile.fCost() || openSet[i].fCost() == currentTile.fCost() && openSet[i].hCost < currentTile.hCost) 
			   {
				currentTile = openSet[i];
			   }
			}

			openSet.Remove(currentTile);
			closedSet.Add(currentTile);

			if (currentTile == targetTile) //path has been found
			{
				RetracePath(startTile, targetTile);
				return;
			}

			foreach (SimTile neighbour in grid.GetNeighboursOfType(currentTile, type))
			{
				if (closedSet.Contains(neighbour)) //ignore closed neighbours
				{
					continue;
				}

				int newMovementCostToNeighbour = currentTile.gCost + GetDistance(currentTile, neighbour);
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
		int dstX = Mathf.Abs(tileA.gridX - tileB.gridX);
		int dstY = Mathf.Abs(tileA.gridY - tileB.gridY);

		return dstX + dstY; //since we dont move diagonally
	}

}
