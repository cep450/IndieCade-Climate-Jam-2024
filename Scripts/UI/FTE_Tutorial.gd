extends Control

@onready var images = [
	$pop_up1,
	$pop_up2,
	$pop_up3,
	$pop_up4,
	$pop_up5,
	$pop_up6,
	$pop_up7
]  

@onready var next_button = $Full_Screen_Next_Button
var mainScene = "res://Scenes/main.tscn"

var current_index = 0

func _ready():
	_update_image_visibility()
	next_button.connect("pressed", Callable(self, "_on_next_button_pressed"))

func _on_next_button_pressed():
	current_index += 1
	if current_index >= images.size():
		current_index = -1  # go to main.tscn
	_update_image_visibility()

func _update_image_visibility():
	for i in range(images.size()):
		images[i].visible = (i == current_index)
		
	if current_index == -1:
		get_tree().change_scene_to_file(mainScene)
