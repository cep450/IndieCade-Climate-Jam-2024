extends Node3D

var data = []
var tile = preload("res://Scenes/tile.tscn")
enum  TileType
{
	HOUSE,
	WORK,
	ROAD,
	PARK,
}

func _ready():
	data.append([TileType.HOUSE,TileType.ROAD,TileType.PARK])
	data.append([TileType.PARK,TileType.ROAD,TileType.PARK])
	data.append([TileType.PARK,TileType.ROAD,TileType.WORK])
	var pos = Vector3(0,0,0)
	for i in 10:
		for j in 10: 
			var instance = tile.instantiate()
			add_child(instance)
			instance.position = pos
			pos.x += 2
		pos.z += 2
		pos.x -= 20
