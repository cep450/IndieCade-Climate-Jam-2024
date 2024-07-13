extends Node3D

var data = []
var tile = preload("res://Scenes/view_tile.tscn")
signal tile_clicked(x: int, y: int)

# The data here is a substitute for Simulation
func _ready():
	var pos = Vector3(-4.5,0,-4.5)
	for x in 10:
		for y in 10: 
			var instance = tile.instantiate()
			add_child(instance)
			instance.initialize(x,y)
			instance.position = pos
			if (y % 2 != 0):
				instance.test_init("Road")
			else:
				instance.test_init("House")
			pos.x += 1
		pos.z += 1
		pos.x -= 10