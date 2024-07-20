extends Control

@onready var images = [
	$TextureRect,
	$TextureRect2,
	$TextureRect3,
	$TextureRect4,
	$TextureRect5,
	$TextureRect6,
	$TextureRect7
]  

@onready var next_button = $NextButton

var current_index = 0

func _ready():
	_update_image_visibility()
	next_button.connect("pressed", Callable(self, "_on_next_button_pressed"))

func _on_next_button_pressed():
	current_index += 1
	if current_index >= images.size():
		current_index = -1  # make all images invisible
	_update_image_visibility()

func _update_image_visibility():
	for i in range(images.size()):
		images[i].visible = (i == current_index)
		
	if current_index == -1:
		for i in range(images.size()):
			images[i].visible = false
