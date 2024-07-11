extends Control

# Clickable components connect their signals to this main UI script when created.
# Functions in this main UI script apply changes to the simulation.

# Connections
@export var progress_bar: ProgressBar

# Private Variables
var sim
var emissions_meter

func _ready():
	sim = $"../Simulation"
	emissions_meter = sim.get_node("SimEmissionsMeter")
	progress_bar.max_value = emissions_meter.GetEmissionsCap()

func _process(_delta):
	progress_bar.value += emissions_meter.GetEmissions()
