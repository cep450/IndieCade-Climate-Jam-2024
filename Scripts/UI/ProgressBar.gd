extends ProgressBar

func _ready():
	max_value = 300#get_parent().get_node("%Simulation").getMaxEmissions()
	value = 100
