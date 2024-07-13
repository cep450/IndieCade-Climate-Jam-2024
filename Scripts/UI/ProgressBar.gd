extends ProgressBar

@export var root: Control

func _ready():
	max_value = root.get_node("../Simulation").SayHi()
	value = 100

