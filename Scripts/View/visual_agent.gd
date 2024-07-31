extends Node3D

# approach 1: each agent has a model of each type that they can turn on and off

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

# TODO we'll want to export these probably 
static var modelsPedestrian = ["res://Models/Agent_Person/Agent_Animated/agent_person_1.tscn", "res://Models/Agent_Person/Agent_Animated/agent_person_2.tscn", "res://Models/Agent_Person/Agent_Animated/agent_person_3.tscn", "res://Models/Agent_Person/Agent_Animated/agent_person_4.tscn"]
static var modelsCyclist = ["res://Models/Agent_Biker/agent_biker_1.tscn"]
static var modelsCars = ["res://Models/Car/Car_Yellow_Driving.tscn", "res://Models/Car/Car_Blue_Driving.tscn", "res://Models/Car/Car_Green_Driving.tscn", "res://Models/Car/car_red_driving.tscn"]
var models = []

func _init():
	visible = false

	#TODO make it so arbitrary modes are possible and it uses paths from SimVehicleTypes instead of hardcoding & can pick randomly from a list with probability weights
	
	var randP = randi_range(0, modelsPedestrian.size() - 1)
	var randCy = randi_range(0, modelsCyclist.size() - 1)
	var randCa = randi_range(0, modelsCars.size() - 1)
	
	models = [
		load(modelsPedestrian[randP]).instantiate(),
		load(modelsCars[randCa]).instantiate(),
		load(modelsCyclist[randCy]).instantiate()
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
		if(position != posTo):
			look_at(posTo)
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
