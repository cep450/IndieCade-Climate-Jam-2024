extends Button

var sim

# Change to the enum later
func initialize(param_type):
	sim = get_parent().get_parent().sim
	match param_type:
		Global.InfraType.ROAD:
			text = "road"
		Global.InfraType.HOUSE:
			text = "house"
		Global.InfraType.BUILDING:
			text = "building"

func _on_pressed():
	#add infra of the appropriate type
	pass
	
