extends Node

var inDevMode: bool = true

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
