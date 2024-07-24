

extends Control

@onready var start_button = $VBoxContainer/StartButton
@onready var exit_button = $VBoxContainer/QuitButton
var tutorial = preload("res://Scenes/FTE_Tutorial.tscn") as PackedScene
var start_game = preload("res://Scenes/main.tscn") as PackedScene

func _ready():
	$VBoxContainer/StartButton.pressed.connect(on_start_pressed)
	$VBoxContainer/TutorialButton.pressed.connect(on_tutorial_pressed)
	$VBoxContainer/QuitButton.pressed.connect(on_quit_pressed)
	

func on_start_pressed() -> void:
	get_tree().change_scene_to_packed(start_game)

func on_tutorial_pressed():
	get_tree().change_scene_to_packed(tutorial)

func on_quit_pressed() -> void:
	get_tree().quit()