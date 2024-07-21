extends Button

# Public
signal entered

# Private
var type: SimInfraType
var is_build: bool 
var bypass_restrictions: bool

@onready var sim = Global.sim
var x: int
var y: int
# Change to the enum later
func initialize(param_type: SimInfraType, param_x: int, param_y: int) -> void:
	bypass_restrictions = Global.inDevMode
	x = param_x
	y = param_y
	if (param_type == null):
		initialize_as_remove_button()
		return
	is_build = true
	type = param_type
	icon = type.Icon

func initialize_as_remove_button():
	icon = load("res://Assets/Gameplay/Menu/Build/Build Menu - Back Icon.png")
	is_build = false

#just adds houses for now
func _on_pressed():
	if (is_build):
		var error = sim.GetTile(x,y).AddInfra(type,bypass_restrictions, true, true)
		if error != "":
			entered.emit(error)
	else:
		# Clear all infra
		print(str(x) + " , " + str(y))
		for t in sim.GetInfra(x,y):
			sim.GetTile(x,y).RemoveInfra(t,bypass_restrictions,true,true);
		
func _on_mouse_entered():
	var return_val
	if is_build:
		return_val = type.Name + " - " + str(type.costToBuild)
	else:
		var total_cost = 0
		for t in sim.GetInfra(x,y):
			total_cost += t.costToDestroy
		return_val = "Destroy All - " + str(total_cost)
	entered.emit(return_val)
	

