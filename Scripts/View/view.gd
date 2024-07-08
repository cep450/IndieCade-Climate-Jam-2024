extends Node3D

var data = []
var tile = preload("res://Scenes/tile.tscn")

# The data here is a substitute for Simulation

func _ready():
	var pos = Vector3(-10,0,-10)
	for i in 10:
		for j in 10: 
			var instance = tile.instantiate()
			add_child(instance)
			instance.initialize(0,0)
			instance.position = pos
			if (j % 2 == 0):
				instance.test_init("Road")
			else:
				instance.test_init("House")
			pos.x += 1
		pos.z += 1
		pos.x -= 10
