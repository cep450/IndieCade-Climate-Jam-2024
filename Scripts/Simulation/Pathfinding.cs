using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;

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
	PathVertex [,] parents;
	PathEdge [,] parentEdges;
	int width, height;

	public void Init() {

		width = Sim.Instance.PathGraph.Width;
		height = Sim.Instance.PathGraph.Height;

		weights = new float[width, height];
		visited = new bool[width, height];
		parents = new PathVertex[width, height];
		parentEdges = new PathEdge[width, height];
		
		initialized = true;
	}

	//TODO rework this when we get agent vehicle transfer 
	// because this SUCKS!
	public SimPath FindPath(PathVertex startVert, SimInfraType.DestinationType destinationType, SimAgent agent) {
		SimPath path = null; 
		foreach(SimVehicleType.TransportMode mode in Enum.GetValues<SimVehicleType.TransportMode>()) {
			SimPath newPath = FindPath(startVert, destinationType, agent, mode);
			GD.Print(" path was " + (newPath == null));
			if(path == null || newPath.totalWeight < path.totalWeight) {
				path = newPath;
			}
		}
		return path;
	}


	//TODO rework this so agents don't use a single type of infrastructure to find a path- let them transfer between them
	SimPath FindPath(PathVertex startVert, SimInfraType.DestinationType destinationType, SimAgent agent, SimVehicleType.TransportMode mode)
	{
		if(!initialized) Init();

		//GD.Print("called FindPath");

		// Run Dijkstra's until we land on a vert of the specified destination type. Then, return that path.

		// set up default values 
		for(int x = 0; x < width; x++) {
			for(int y = 0; y < height; y++) {
				weights[x,y] = float.PositiveInfinity;
				visited[x,y] = false;
				parents[x,y] = null;
			}
		}

		// set up source 
		weights[startVert.PathGraphCoordinates.X, startVert.PathGraphCoordinates.Y] = 0f;

		List<PathVertex> openSet = new List<PathVertex>();
		openSet.Add(startVert);

		bool foundDestination = false;
		while(openSet.Count > 0 && !foundDestination) {

			GD.Print(" open set size " + openSet.Count);

			// pick min weight next vertex from the set of verts not yet processed 
			PathVertex v = openSet[0];
			float minWeight = float.PositiveInfinity;

			// from the last one to the next
			for(int i = 0; i < openSet.Count; i++) {
				float newWeight = weights[openSet[i].PathGraphCoordinates.X, openSet[i].PathGraphCoordinates.Y];
				if(newWeight < minWeight) {
					minWeight = newWeight;
					v = openSet[i];
				}
			}

			openSet.Remove(v);
			visited[v.PathGraphCoordinates.X, v.PathGraphCoordinates.Y] = true;

			if(v.DestinationType == destinationType) {
				foundDestination = true;
				return RetracePath(startVert, v, agent, mode);
			}

			// add the connected neighbors to the open set 
			foreach(PathEdge edge in v.Edges) {

				//TODO make transit mode switching possible by checking for CanTransfer and updating some kind of local transit mode or making different vertices per mode 
				// maybe we can use sparse arrays for mode-specific vertices and then the tiles keep track of how they link up with edges idk 
				if(edge.TransportMode != mode) {
					continue;
				}

				PathVertex other = edge.GetOther(v);

				float cost = weights[v.PathGraphCoordinates.X, v.PathGraphCoordinates.Y] + agent.WeightConnection(edge, mode);

				int otherX = other.PathGraphCoordinates.X;
				int otherY = other.PathGraphCoordinates.Y;

				// ignore closed neighbors 
				if(cost < weights[otherX, otherY] || visited[otherX, otherY] != true) {
					
					weights[otherX, otherY] = cost;
					parents[otherX, otherY] = v;
					parentEdges[otherX, otherY] = edge;

					if(visited[other.PathGraphCoordinates.X, other.PathGraphCoordinates.Y] != true) {
						openSet.Add(other);
					}
				}

			}

		}

		// we didn't find the destination -- no path 
		return null;

		//TODO switching between transit modes.
		// each edge represents a single transit mode 
		// so..... 

		//TODO the problem we need to fix with transit modes is a vert can be added to the closed set if one transit mode hits it, which closes it off for all the others.

		//TODO also determining the accessible transit modes from the first tile.

	}

	SimPath RetracePath(PathVertex startVert, PathVertex endVert, SimAgent agent, SimVehicleType.TransportMode mode)
	{
		SimPath path = new SimPath();
		PathVertex currentVert = endVert;

		while (currentVert != startVert)
		{
			path.vertices.Add(currentVert);

			PathEdge edge = parentEdges[currentVert.PathGraphCoordinates.X, currentVert.PathGraphCoordinates.Y];
			path.edges.Add(edge);
			path.totalWeight += agent.WeightConnection(edge, mode);
			path.totalSupport += agent.SupportGainedConnection(edge, mode);

			currentVert = parents[currentVert.PathGraphCoordinates.X, currentVert.PathGraphCoordinates.Y];
		}
		//TODO does the start vert need to be in the path? if so, add it here 
		path.vertices.Reverse();
		path.edges.Reverse();

		path.pathVehicleType = SimVehicleType.TypeFromEnum(SimVehicleType.TransportMode.CAR);
		path.totalSupport += agent.suppLumpSumTransportMode[(int)mode];

		return path;
	}

//TODO we could move this to SimGrid if we need it in the future 
/*
	int GetDistance(SimTile tileA, SimTile tileB)
	{
		int dstX = Mathf.Abs(tileA.Coordinates.X - tileB.Coordinates.X);
		int dstY = Mathf.Abs(tileA.Coordinates.Y - tileB.Coordinates.Y);

		return dstX + dstY; //since we dont move diagonally
	}*/

}
