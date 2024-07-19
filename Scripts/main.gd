extends Node

func tick():
	for child in get_children():
		if child.has_method("tick"):
			child.tick()
