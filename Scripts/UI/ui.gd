extends Control

# Clickable components connect their signals to this main UI script when created.
# Functions in this main UI script apply changes to the simulation.

# Connections
@export var emissions_meter: TextureProgressBar
@export var build_buttons: HBoxContainer

@onready var speed_controller: PanelContainer = $SpeedController
@export var speed_controller_images: Array[Texture2D]

@onready var support_meter: VBoxContainer = $"SupportMeter"
@export var support_meter_images: Array[Texture2D]

@onready var timer = $Timer

@onready var world: Node3D = $"../View/World"
#Public Variables
var support_state: SupportState
var speed: SpeedState

# Private Variables
@onready var sim = Global.sim
@onready var sim_emissions_meter = sim.get_node("SimEmissionsMeter")

var button = preload("res://Scenes/UI/build_button.tscn")	

enum SpeedState 
{
	PAUSE, PLAY, SPEEDUP,
}

enum SupportState
{
	GREAT, NEUTRAL, BAD
}

func _ready():
	speed = SpeedState.PLAY
	build_buttons.get_parent().visible = false
	emissions_meter.get_node("HoverInfo").visible = false
	speed_controller.get_node("Image").texture = speed_controller_images[speed]
	emissions_meter.max_value = sim_emissions_meter.GetEmissionsCap()
	world.tile_clicked.connect(on_tile_clicked)
	$BuildButtons/BuildMenu.mouse_exited.connect(_on_mouse_exited)
		
func _process(_delta):
	# Update Emissions Meter
	emissions_meter.value += sim_emissions_meter.GetEmissions()
	var percent = int(100 * emissions_meter.value/emissions_meter.max_value)
	# Update Emissions HoverInfo
	var percent_change = int(100 * sim_emissions_meter.EmissionRate/emissions_meter.max_value)
	emissions_meter.get_node("HoverInfo/Text").text = str(percent_change) + "%/30s"
	emissions_meter.get_node("Percentage").text = str(percent) + "%"
	# Get Support Info (i.e. repurposed happiness meter)
	var support = sim.get_node("SimSupportPool").Support
	support_meter.get_node("Text").text = str(support)
	if support > 20:
		support_state =  SupportState.GREAT
	elif support > 10:
		support_state = SupportState.NEUTRAL
	else:
		support_state = SupportState.BAD
	support_meter.get_node("Image").texture = support_meter_images[support_state]
	# Get clock
	# TODO not sure exactly how we want this done

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
		var types = load("res://Resources/InfraTypes/bikelane.tres").GetTypes()
		for type in types:
			if type.Icon != null:
				var instance = button.instantiate()
				build_buttons.add_child(instance)
				instance.initialize(type, x, y)
				instance.entered.connect(_on_mouse_entered_build)
		return

func _on_emissions_meter_mouse_entered():
	emissions_meter.get_node("HoverInfo").visible = true

func _on_emissions_meter_mouse_exited():
	emissions_meter.get_node("HoverInfo").visible = false

func _on_mouse_entered_build(text: String):
	$BuildInfo.visible = true
	$BuildInfo/Text.text = text
	$BuildInfo.size.x = $BuildButtons.size.x
	$BuildInfo.position.x = $BuildButtons.position.x

func _on_mouse_exited():
	$BuildInfo.visible = false
