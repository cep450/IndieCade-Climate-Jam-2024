extends Control

# Clickable components connect their signals to this main UI script when created.
# Functions in this main UI script apply changes to the simulation.

# Connections
@export var emissions_meter: TextureProgressBar
@export var build_buttons: HBoxContainer

@export var speed_controller: PanelContainer
@export var speed_controller_images: Array[Texture2D]

@export var happiness_meter_images: Array[Texture2D]

#Public Variables
var happiness: HappinessState
var speed: SpeedState

# Private Variables
@onready var sim = Global.sim
@onready var sim_emissions_meter = sim.get_node("SimEmissionsMeter")

var button = preload("res://Scenes/UI/build_button.tscn")	

enum SpeedState 
{
	PAUSE, PLAY, SPEEDUP,
}

enum HappinessState
{
	GREAT, NEUTRAL, BAD
}

func _ready():
	speed = SpeedState.PLAY
	speed_controller.get_node("Image").texture = speed_controller_images[speed]
	emissions_meter.max_value = sim_emissions_meter.GetEmissionsCap()
	$"../View/World".tile_clicked.connect(on_tile_clicked)

func _process(_delta):
	# Update Emissions Meter
	emissions_meter.value += sim_emissions_meter.GetEmissions()
	var percent = int(100 * emissions_meter.value/emissions_meter.max_value)
	emissions_meter.get_node("Percentage").text = str(percent) + "%"
	# Update Happines State
	
	
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
	
