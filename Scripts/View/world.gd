extends Node3D

var tile = preload("res://Scenes/View/view_tile.tscn")
signal tile_clicked(x: int, y: int)
@onready var sim: Node = Global.sim

func init_tile(x, y, xpos, ypos):
	var instance = tile.instantiate()
	add_child(instance)
	instance.initialize(x,y)
	instance.position = Vector3(xpos, 0, ypos)
	return instance
	
func update_all_tile_visuals():
	for child in get_children():
		if (child.has_method("update_visuals")):
			child.update_visuals(true)
