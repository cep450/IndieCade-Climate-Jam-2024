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

# TODO this is kinda inefficient but don't have time to optimize
func update_neighbors(coord: Vector2i):
	for child in get_children():
		if child.has_method("update_visuals"):
			# If orthogonally adjacent.
			if abs(child.x - coord.x) + abs(child.y - coord.y) == 1:
				child.update_visuals(true)
