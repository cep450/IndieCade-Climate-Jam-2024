using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;

public partial class Pathfinding : Node
{

	/*
		Create a SimPath to give to a SimAgent.
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

	public SimPath FindPath(PathVertex startVert, SimInfraType.DestinationType destinationType, SimAgent agent) {
		return FindPath(startVert, destinationType, agent, Vector2I.MinValue);
	}

	//TODO rework this when we get agent vehicle transfer working
	// because this SUCKS!
	public SimPath FindPath(PathVertex startVert, SimInfraType.DestinationType destinationType, SimAgent agent, Vector2I coordinates) {
		
		SimPath path = null; 
		
		foreach(SimVehicleType.TransportMode mode in Enum.GetValues<SimVehicleType.TransportMode>()) {
			SimPath newPath = FindPath(startVert, destinationType, agent, coordinates, mode);
			if(newPath == null) continue;
			if(path == null || newPath.totalWeight < path.totalWeight) {
				path = newPath;
			}
		}
		if(path == null) GD.Print("no path found to " + destinationType + " from " + startVert.PathGraphCoordinates.ToString());
		return path;
	}


	//TODO rework this so agents don't use a single type of infrastructure to find a path- let them transfer between them
	//TODO this looks for the first vert that meets the requirements, which is good for a generic search for the closest vertex of a type. But we'll want to do a slightly different impementation for pathfinding to homes with known coordinates, we can probably use A* or something similar
	SimPath FindPath(PathVertex startVert, SimInfraType.DestinationType destinationType, SimAgent agent, Vector2I coordinates, SimVehicleType.TransportMode mode)
	{
		if(!initialized) Init();

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

			//GD.Print(" open set size " + openSet.Count);

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

				// when we're looking for specific coordinates, like travelling back home 
				if(coordinates != Vector2I.MinValue) {
					if(coordinates == v.PathGraphCoordinates) {
						foundDestination = true; 
						return RetracePath(startVert, v, agent, mode);
					}
				}

				// when we're just trying to find the first available destination of a type 
				if(v.TryAddOccupancy(SimVehicleType.TransportMode.PEDESTRIAN)) {
					foundDestination = true;
					return RetracePath(startVert, v, agent, mode);
				} else {
					continue;
				}
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

					if(visited[otherX, otherY] != true && !openSet.Contains(other)) {
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

		path.pathVehicleType = SimVehicleType.TypeFromEnum(mode);

		//TODO non bullshit way 
		if(mode == SimVehicleType.TransportMode.PEDESTRIAN) {
			path.totalSupport += agent.suppLumpSumTransportMode[0];
		} else if(mode == SimVehicleType.TransportMode.CAR) {
			path.totalSupport += agent.suppLumpSumTransportMode[1];
		} else if(mode == SimVehicleType.TransportMode.BIKE) {
			path.totalSupport += agent.suppLumpSumTransportMode[2];
		}

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
