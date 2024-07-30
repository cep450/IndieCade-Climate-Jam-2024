extends Control

@onready var restart_button = $TextureRect/Restart_Button
@onready var exit_button = $TextureRect/Exit_Button

var mainScene = "res://Scenes/main.tscn"


func _ready():

	restart_button.connect("pressed", Callable(self, "_on_restart_button_pressed"))
	exit_button.connect("pressed", Callable(self, "_on_exit_button_pressed"))

func _on_restart_button_pressed():
	get_tree().change_scene_to_file(mainScene)
	
func _on_exit_button_pressed():
	get_tree().quit()
