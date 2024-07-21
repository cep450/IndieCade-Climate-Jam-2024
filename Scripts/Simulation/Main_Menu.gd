

extends Control

@onready var start_button = $StartButton
@onready var exit_button = $QuitButton
@onready var start_level = preload("res://Scenes/FTE_Tutorial.tscn") as PackedScene

func _ready():
	start_button.button_down.connect(on_start_pressed)
#	tutorial_button.button_down.connect(on_tutorial_pressed)
	exit_button.button_down.connect(on_exit_pressed)
	

func on_start_pressed() -> void:
	get_tree().change_scene_to_packed(start_level)

func on_exit_pressed() -> void:
	get_tree().quit()
