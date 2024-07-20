extends Node3D

var tile = preload("res://Scenes/View/view_tile.tscn")
var agent = preload("res://Scenes/View/visual_agent.tscn")
var agent_count = 0
signal tile_clicked(x: int, y: int)
@onready var sim: Node = Global.sim

func init_tile(x, y, xpos, ypos):
	var instance = tile.instantiate()
	add_child(instance)
	instance.initialize(x,y)
	instance.position = Vector3(xpos, 0, ypos)
	instance.name = "Tile" + str(x) + str(y)
	return instance
	
func init_agent():
	var instance = agent.instantiate()
	add_child(instance)
	instance.position = Vector3(0,2,0)
	instance.name = "Agent" + str(agent_count)
	agent_count += 1
	return instance;

# TODO this is kinda inefficient but don't have time to optimize
func update_neighbors(coord: Vector2i):
	for child in get_children():
		if child.has_method("update_visuals"):
			# If orthogonally adjacent.
			if abs(child.x - coord.x) + abs(child.y - coord.y) == 1:
				child.update_visuals(true)

