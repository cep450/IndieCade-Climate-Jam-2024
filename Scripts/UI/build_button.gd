extends Button


@export var images: Array[Texture2D] = []

# Private
var type
var sim
var x: int
var y: int
# Change to the enum later
func initialize(param_type, param_x, param_y):
	type = param_type
	x = param_x
	y = param_y
	sim = get_tree().root.get_node("Main/UI").sim
	icon = images[param_type]

func _on_pressed():
	sim.SayHi()
	sim.AddInfra()
	

