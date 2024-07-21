extends Node

var sim: Node
@export var inDevMode: bool = false

var current_tile: Vector2 = Vector2(-1, -1)

# Called by view_tile, Vector2 stores an index
func on_tile_clicked(clicked_tile: Vector2):
	if clicked_tile == current_tile:
		current_tile = Vector2(-1,-1)
	else: 
		current_tile = clicked_tile

func set_sim(sim_node: Node):
	sim = sim_node

func say_hi():
	print("hi")
