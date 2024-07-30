extends Node3D

# approach 1: each agent has a model of each type that they can turn on and off
var models

# approach 2: using an object pooler

func _init():
	visible = false

	#TODO make it so arbitrary modes are possible and it uses paths from SimVehicleTypes instead of hardcoding 
	models = [
		load("res://Models/Car/Car_Yellow_Driving.tscn").instantiate(),
		load("res://Models/Car/Car_Yellow_Driving.tscn").instantiate(),
		load("res://Models/Car/Car_Yellow_Driving.tscn").instantiate()
	]
	for m in models:
		add_child(m)
		m.hide()


func Set_Pos(pos: Vector3):
	position = pos

#func Set_Vehicle(_path: String):
func Set_Vehicle(index: int):
	for m in models:
		m.hide()
	models[index].show()
	#for child in get_children():
	#	child.queue_free()
	#if path != "":
		#print("creating vehicle")
		#var instance = load(path).instantiate()
		#add_child(instance)


func Set_Visible(is_visible_b: bool):
	visible = is_visible_b
