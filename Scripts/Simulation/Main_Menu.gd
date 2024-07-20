

extends Control


#@onready var start_button = $MarginContainer/HBoxContainer/VBoxContainer/Start_Button as Button
#@onready var tutorial_button = $MarginContainer/HBoxContainer/VBoxContainer/Tutorial_Button as Button
#@onready var start_level = preload("res://Scenes/main.tscn") as PackedScene
#@onready var exit_button = $MarginContainer/HBoxContainer/VBoxContainer/Exit_Button as Button


# Called when the node enters the scene tree for the first time.
#func _ready():
	#start_button.button_down.connect(on_start_pressed)
	#tutorial_button.button_down.connect(on_tutorial_pressed)
	#exit_button.button_down.connect(on_exit_pressed)
	

#func on_start_pressed() -> void:
	#get_tree().change_scene_to_packed(start_level)

#func on_tutorial_pressed() -> void:
	#pass
	
#func on_exit_pressed() -> void:
	#get_tree().quit()
	
	


@onready var start_button = $StartButton
@onready var exit_button = $QuitButton
@onready var start_level = preload("res://Scenes/main.tscn") as PackedScene

func _ready():
	start_button.button_down.connect(on_start_pressed)
#	tutorial_button.button_down.connect(on_tutorial_pressed)
	exit_button.button_down.connect(on_exit_pressed)
	

func on_start_pressed() -> void:
	get_tree().change_scene_to_packed(start_level)

func on_exit_pressed() -> void:
	get_tree().quit()

#func _ready():
	# Ensure the start button is receiving input events
	#start_button.connect("gui_input", Callable(self, "_on_start_button_input"))

#func _on_start_button_input(event):
	#if event is InputEventMouseButton and event.pressed:
		#if event.button_index == MOUSE_BUTTON_LEFT:
			#if start_button.get_rect().has_point(start_button.get_local_mouse_position()):
#				print("Input Event Detected on Start Button")
#				_on_start_button_pressed()

#func _on_start_button_pressed():
#	print("Start button pressed!")
#	get_tree().change_scene_to_packed(start_level)
