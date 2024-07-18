extends Node3D

var data = []
var tile = preload("res://Scenes/View/view_tile.tscn")
signal tile_clicked(x: int, y: int)
var sim: Node

# The data here is a substitute for Simulation
func _ready():
	sim = $"../../Simulation"

func update_all_tiles_visuals():
	pass
	
func update_eelected_tile_visuals():
	pass

func init_tile(x, y, xpos, ypos):
	if(sim == null):
		sim = $"../../Simulation"
	var instance = tile.instantiate()
	add_child(instance)
	instance.initialize(x,y)
	instance.position = Vector3(xpos, 0, ypos)
	instance.test_init("Blank")
	return instance;
