extends Node3D

# approach 1: each agent has a model of each type that they can turn on and off
var models

# approach 2: using an object pooler
# TODO if approach 1 is still too much of a performance impact 

# smooth movement variables 
var posFrom
var posTo
var moveVector
var moving = false
var speed = 0
var maxSpeed
var acceleration
var percentTravelled = 0
var distance
var speedDecayRate = 1

var simAgent

var speedMultiplier = 0.02

func _init():
	visible = false

	#TODO make it so arbitrary modes are possible and it uses paths from SimVehicleTypes instead of hardcoding & can pick randomly from a list with probability weights
	models = [
		load("res://Models/Agent_Person/Agent_Person_1.gltf").instantiate(),
		load("res://Models/Car/Car_Yellow_Driving.tscn").instantiate(),
		load("res://Models/Agent_Biker/Agent_Biker_1.gltf").instantiate()
	]
	for m in models:
		add_child(m)
		m.hide()

func _process(delta):
	# Smooth movement, if we are moving. 
	#TODO try doing some easing curves for initial acceleration and deceleration
	if(moving):
		if(speed < maxSpeed):
			speed = min(maxSpeed, speed + (acceleration * delta))
		percentTravelled = percentTravelled + (delta * speedMultiplier * speed / distance)
		percentTravelled = min(percentTravelled, 1)
		position = lerp(posFrom, posTo, percentTravelled)
		if(percentTravelled == 1):
			moving = false
			simAgent.MoveNext()
			return
	else:
		speed = max(0, speed - (speedDecayRate * delta))	

# Hard set position with no movement.
func Set_Pos(pos: Vector3):
	position = pos

func Move(posStart: Vector3, posEnd: Vector3, _maxSpeed: float, _acceleration: float, _decceleration: float):
	position = posStart
	posFrom = posStart
	posTo = posEnd 
	maxSpeed = _maxSpeed
	acceleration = _acceleration
	speedDecayRate = _decceleration
	moveVector = (posEnd - posStart).normalized()
	distance = (posEnd - posStart).length()

	#rotate model to direction facing 
	print(moveVector)

	percentTravelled = 0
	moving = true 


func Arrived():
	Set_Visible(false)
	speed = 0

#func Set_Vehicle(_path: String):
func Set_Vehicle(index: int):
	for m in models:
		m.hide()
	models[index].show()

func Set_Visible(is_visible_b: bool):
	visible = is_visible_b
