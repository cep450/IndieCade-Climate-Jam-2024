extends Button

# Public
signal entered

# Private
var type
var is_build: bool 
var bypass_restrictions: bool
var trees_res = load("res://Resources/InfraTypes/tree.tres")
var grass_res = load("res://Resources/InfraTypes/grass.tres")
var concrete_res = load("res://Resources/InfraTypes/concrete.tres")
var parking_lot_res = load("res://Resources/InfraTypes/parking_lot.tres")

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
		#Special code for tree replacement
		if type == trees_res:
			clear_infra()
			# Try to add tree
			if sim.GetTile(x,y).AddInfra(trees_res,bypass_restrictions, true, true) == "":
				# If you can add the grass tile as well
				sim.GetTile(x,y).AddInfra(grass_res,true, true, true)
			
		var error = sim.GetTile(x,y).AddInfra(type,bypass_restrictions, true, true)
		if error != "":
			entered.emit(error)
	else:
		clear_infra()
		
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

# Helper functions
func clear_infra():
	# Clear all infra
	for t in sim.GetInfra(x,y):
		sim.GetTile(x,y).RemoveInfra(t,true,true,true);

