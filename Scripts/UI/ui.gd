extends Control

# Clickable components connect their signals to this main UI script when created.
# Functions in this main UI script apply changes to the simulation.

# Connections
@export var emissions_meter: TextureProgressBar
@export var build_buttons: HBoxContainer

@onready var speed_controller: PanelContainer = $SpeedController
@export var speed_controller_images: Array[Texture2D]

@onready var happiness_meter: VBoxContainer = $"HappinessMeter"
@export var happiness_meter_images: Array[Texture2D]

@onready var world: Node3D = $"../View/World"
#Public Variables
var happiness_state: HappinessState
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

func tick():
	pass

func _ready():
	speed = SpeedState.PLAY
	build_buttons.get_parent().visible = false
	speed_controller.get_node("Image").texture = speed_controller_images[speed]
	emissions_meter.max_value = sim_emissions_meter.GetEmissionsCap()
	world.tile_clicked.connect(on_tile_clicked)

func _process(_delta):
	# Update Emissions Meter
	emissions_meter.value += sim_emissions_meter.GetEmissions()
	var percent = int(100 * emissions_meter.value/emissions_meter.max_value)
	emissions_meter.get_node("Percentage").text = str(percent) + "%"
	# Update Happines State
	# TODO request this info from simulation.
	# repurposing this for Support since we condensed Support and Happiness into one number for the jam
	#var happiness = sim.SupportPool.GlobalHappiness
	var support = 10#sim.get_node("SimSupportPool").Support
	happiness_meter.get_node("Text").text = str(support)
	if support > 20:
		happiness_state =  HappinessState.GREAT
	elif support > 10:
		happiness_state = HappinessState.NEUTRAL
	else:
		happiness_state = HappinessState.BAD
	happiness_meter.get_node("Image").texture = happiness_meter_images[happiness_state]
	# Update Support
	#$SupportTemp.text = "Support: " + str(support)
	
func on_tile_clicked(x: int, y: int) -> void:
	# Only display if tile selected checked by if it's a valid index.
	if Global.current_tile == Vector2(-1,-1):
		build_buttons.get_parent().visible = false
		return
	else:
		build_buttons.get_parent().visible = true
		# Follow in the footsteps of Anikin and delete all the children
		for child in build_buttons.get_children():
			child.queue_free()
		# Temp func to add test ones
		for type in 1:
			var instance = button.instantiate()
			build_buttons.add_child(instance)
			instance.initialize(load("res://Resources/InfraTypes/road.tres"), x, y)
		return
	
