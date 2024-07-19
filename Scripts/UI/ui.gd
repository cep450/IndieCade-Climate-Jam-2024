extends Control

# Clickable components connect their signals to this main UI script when created.
# Functions in this main UI script apply changes to the simulation.

# Connections
@export var emissions_meter: TextureProgressBar
@export var build_buttons: HBoxContainer

# Private Variables
@onready var sim = Global.sim
@onready var sim_emissions_meter = sim.get_node("SimEmissionsMeter")

var button = preload("res://Scenes/UI/build_button.tscn")	


func _ready():
	emissions_meter.max_value = sim_emissions_meter.GetEmissionsCap()
	$"../View/World".tile_clicked.connect(on_tile_clicked)

func _process(_delta):
	emissions_meter.value += 10 
	var percent = int(100 * emissions_meter.value/emissions_meter.max_value)
	emissions_meter.get_node("Percentage").text = str(percent) + "%"
	#emissions_meter.value += emissions_meter.GetEmissions()
	
func on_tile_clicked(x: int, y: int) -> void:
	# Follow in the footsteps of Anikin and delete all the children
	for child in build_buttons.get_children():
		child.queue_free()
	# Temp func to add test ones
	var types = [Global.InfraType.WORK]
	for type in types:
		var instance = button.instantiate()
		build_buttons.add_child(instance)
		instance.initialize(load("res://Resources/InfraTypes/road.tres"), x, y)
	return
