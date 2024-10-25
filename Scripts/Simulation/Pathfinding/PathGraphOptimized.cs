using Godot;
using System;
using System.Collections.Generic;

public class PathGraphOptimized {

	// We'll use graph theory to take the PathGraph, which stores every connection between every vertex based on every tile on the map,
	// and use it to generate an optimized graph to pathfind on 
	// for example: if there's a 10 tile long road in a straight line, we say that's one edge from the vertex at the start and the vertex at the end 

	/* 
	 *	TODO: 
	 *  - determine when to generate a new optimized graph. when the player makes changes to the map? when construction completes? with a delay to prevent constant rebuilding?
	 *  - only rebuild the parts of the graph that have changed, based on the PathGraph 
	 *	- make PathGraphOptimized's information accessible to the pathfinding logic just like the original PathGraph 
	 *  - figure out how to account for vehicles taking up space on the road. maybe the original PathGraph handles agents moving from place to place moment-to-moment,
	 *    and PathGraphOptimized is used to more quickly generate the path used by the agents, once at the beginning of their trip 
	 *  - how can we store the data most efficiently? maybe both PathGraph and PathGraphOptimized use the same PathVertices and PathEdges, but within those they store a bool if it's being used by the optimized graph or not
	 */
	

	// build the whole graph for the first time e.g. when saving a map file generate the default optimized pathgraph 
	public void BuildGraph() {

	}

	// rebuild part of the graph when a vertex is changed e.g. when a tile is changed 
	public void RebuildGraph(Vector2I coordChanged) {

	}

	// ditto, but batch multiple vertices to update at once 
	public void RebuildGraph(List<Vector2I> coordsChanged) {

	}

}