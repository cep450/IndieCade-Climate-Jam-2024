extends Node

var inDevMode: bool = true
@onready var sim: Node = $"../Main/Simulation"

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

@export var resource: Array[SimInfraType]
