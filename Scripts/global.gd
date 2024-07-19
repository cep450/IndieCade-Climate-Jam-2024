extends Node

var inDevMode: bool = true
@onready var sim: Node = $"../Main/Simulation"
var count: int = 0

enum InfraType {
	HOME,	
	WORK,
	COMMERCIAL,
	THIRDSPACE,
	ROAD,
	SIDEWALK,
	CROSSWALK, 
	BIKELANE,
	STREETLAMP, 
	TREE,
	PARKINGLOT,
	BIKERACK,
}
