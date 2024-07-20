extends Button

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
	var bypass_validation = Global.inDevMode
	sim.GetTile(x,y).AddInfra(type, bypass_validation, true)
	
	

"res://Scenes/Tiles/SidewalkCurve.tscn"
