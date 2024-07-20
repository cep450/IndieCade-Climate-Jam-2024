extends Button

# Public
signal entered
# Private
var type: SimInfraType
@onready var sim = Global.sim
var x: int
var y: int
# Change to the enum later
func initialize(param_type: SimInfraType, param_x: int, param_y: int) -> void:
	type = param_type
	x = param_x
	y = param_y
	icon = type.Icon

#just adds houses for now
func _on_pressed():
	var error = sim.GetTile(x,y).AddInfra(type, false, true)
	if error != "":
		entered.emit(error)
		
func _on_mouse_entered():
	entered.emit(type.Name + " - " + str(type.costToBuild))
