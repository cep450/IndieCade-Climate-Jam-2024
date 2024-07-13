using Godot;
using System;
using System.Collections.Generic;

public partial class SimPath : Node
{

	/*
	 *  A path between two tiles.
	 */

	private Dictionary<Vector2, SimTile> _grid;
	private int _gridSize = 10;

	public override void _Ready()
	{
		// Initialize the grid with SimTile nodes
		_grid = new Dictionary<Vector2, SimTile>();

		for (int x = 0; x < _gridSize; x++)
		{
			for (int y = 0; y < _gridSize; y++)
			{
				Vector2 position = new Vector2(x, y);
				SimTile tile = new SimTile(position);
				_grid[position] = tile;
			}
		}
	}

	public Vector2 GetNextStep(Vector2 start, Vector2 end)
	{
		List<Vector2> path = AStarPathfinding(start, end);

		if (path.Count > 1)
		{
			return path[1]; // Return the next step towards the target
		}

		return start; // If no path found, return the start position
	}

	private List<Vector2> AStarPathfinding(Vector2 start, Vector2 end)
	{
		HashSet<Vector2> closedSet = new HashSet<Vector2>();
		HashSet<Vector2> openSet = new HashSet<Vector2> { start };
		Dictionary<Vector2, Vector2> cameFrom = new Dictionary<Vector2, Vector2>();

		Dictionary<Vector2, float> gScore = new Dictionary<Vector2, float>
		{
			{ start, 0 }
		};

		Dictionary<Vector2, float> fScore = new Dictionary<Vector2, float>
		{
			{ start, HeuristicCostEstimate(start, end) }
		};

		while (openSet.Count > 0)
		{
			Vector2 current = GetNodeWithLowestFScore(openSet, fScore);

			if (current == end)
			{
				return ReconstructPath(cameFrom, current);
			}

			openSet.Remove(current);
			closedSet.Add(current);

			foreach (Vector2 neighbor in GetNeighbors(current))
			{
				if (closedSet.Contains(neighbor))
				{
					continue;
				}

				float tentativeGScore = gScore[current] + Distance(current, neighbor);

				if (!openSet.Contains(neighbor))
				{
					openSet.Add(neighbor);
				}
				else if (tentativeGScore >= gScore[neighbor])
				{
					continue;
				}

				cameFrom[neighbor] = current;
				gScore[neighbor] = tentativeGScore;
				fScore[neighbor] = gScore[neighbor] + HeuristicCostEstimate(neighbor, end);
			}
		}

		return new List<Vector2>(); // Return an empty path if no path found
	}

	private Vector2 GetNodeWithLowestFScore(HashSet<Vector2> openSet, Dictionary<Vector2, float> fScore)
	{
		Vector2 lowest = Vector2.Zero;
		float minScore = float.MaxValue;

		foreach (Vector2 node in openSet)
		{
			if (fScore.ContainsKey(node) && fScore[node] < minScore)
			{
				minScore = fScore[node];
				lowest = node;
			}
		}

		return lowest;
	}

	private List<Vector2> ReconstructPath(Dictionary<Vector2, Vector2> cameFrom, Vector2 current)
	{
		List<Vector2> totalPath = new List<Vector2> { current };

		while (cameFrom.ContainsKey(current))
		{
			current = cameFrom[current];
			totalPath.Insert(0, current);
		}

		return totalPath;
	}

	private float HeuristicCostEstimate(Vector2 start, Vector2 goal)
	{
		return start.DistanceTo(goal);
	}

	private float Distance(Vector2 a, Vector2 b)
	{
		return a.DistanceTo(b);
	}

	private List<Vector2> GetNeighbors(Vector2 node)
	{
		List<Vector2> neighbors = new List<Vector2>();

		Vector2[] directions = new Vector2[]
		{
			new Vector2(1, 0),
			new Vector2(-1, 0),
			new Vector2(0, 1),
			new Vector2(0, -1)
		};

		foreach (Vector2 direction in directions)
		{
			Vector2 neighbor = node + direction;
			if (_grid.ContainsKey(neighbor))
			{
				neighbors.Add(neighbor);
			}
		}

		return neighbors;
	}
}
