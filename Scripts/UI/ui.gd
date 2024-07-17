extends Control

# Clickable components connect their signals to this main UI script when created.
# Functions in this main UI script apply changes to the simulation.

# Connections
@export var emissions_meter: TextureProgressBar
@export var build_buttons: HBoxContainer

# Private Variables
var sim
var sim_emissions_meter

var button = preload("res://Scenes/UI/build_button.tscn")	


func _ready():
	sim = $"../Simulation"
	sim_emissions_meter = sim.get_node("SimEmissionsMeter")
	emissions_meter.max_value = sim_emissions_meter.GetEmissionsCap()
	$"../View/World".tile_clicked.connect(on_tile_clicked)

func _process(_delta):
	emissions_meter.value += 10
	var percent = int(100 * emissions_meter.value/emissions_meter.max_value)
	$EmissionsMeter/Percentage.text = str(percent) + "%"
	#emissions_meter.value += emissions_meter.GetEmissions()
	
func on_tile_clicked(x: int, y: int):
	# Follow in the footsteps of Anikin and delete all the children
	for child in build_buttons.get_children():
		child.queue_free()
	# Temp func to add test ones
	var types = [Global.InfraType.ROAD, Global.InfraType.HOUSE, Global.InfraType.BUILDING]
	for type in types:
		var instance = button.instantiate()
		build_buttons.add_child(instance)
		instance.initialize(type, x, y)
	return
	#var tiles = sim.GetInfra(x,y)
	#for tile in tiles:
		#var instance = button.instantiate()
		#$HBoxContainer.add_child(instance)
	
func sayhi():
	print("hi from UI")
