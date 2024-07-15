extends Node

var inDevMode: bool = true

enum InfraType {
	HOUSE,
	ROAD,
	BUILDING,
}

##@export var resource: Array[SimInfraType]
