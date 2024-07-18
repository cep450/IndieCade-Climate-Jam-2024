extends Button


@export var images: Array[Texture2D] = []

# Private
var type
@onready var sim = Global.sim
var x: int
var y: int
# Change to the enum later
func initialize(param_type, param_x, param_y):
	type = param_type
	x = param_x
	y = param_y
	icon = images[param_type]

#just adds houses for now
func _on_pressed():
	sim.SayHi()
	sim.AddInfra(load("res://Resources/InfraTypes/house.tres"))
	

