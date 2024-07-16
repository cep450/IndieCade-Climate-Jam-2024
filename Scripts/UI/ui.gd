extends Control

# Clickable components connect their signals to this main UI script when created.
# Functions in this main UI script apply changes to the simulation.

# Connections
@export var progress_bar: TextureProgressBar

# Private Variables
var sim
var emissions_meter

var button = preload("res://Scenes/UI/build_button.tscn")	


func _ready():
	sim = $"../Simulation"
	emissions_meter = sim.get_node("SimEmissionsMeter")
	progress_bar.max_value = emissions_meter.GetEmissionsCap()
	$"../View/World".tile_clicked.connect(on_tile_clicked)

func _process(_delta):
	progress_bar.value += 100
	var percent = int(100 * progress_bar.value/progress_bar.max_value)
	$EmissionsMeter/Percentage.text = str(percent) + "%"
	#progress_bar.value += emissions_meter.GetEmissions()
	
func on_tile_clicked(_x: int, _y: int):
	# Follow in the footsteps of Anikin and delete all the children
	
	# I'm commenting this out so the game doesn't crash for people pulling from dev. Uncomment when fixed. --Jaden :^)
	#for child in $BuildButtons.get_children():
	#	child.queue_free()
	# Temp func to add test ones
	#var types = []#[Global.InfraType.ROAD, Global.InfraType.HOUSE, Global.InfraType.BUILDING]
	#for type in types:
	#	var instance = button.instantiate()
	#	$BuildButtons.add_child(instance)
	#	instance.initialize(type)
	return
	#var tiles = sim.GetInfra(x,y)
	#for tile in tiles:
		#var instance = button.instantiate()
		#$HBoxContainer.add_child(instance)
	
