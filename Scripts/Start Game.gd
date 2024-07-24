extends TextureRect

@onready var main_menu = get_parent()  # Adjust the path if necessary

func _ready():
	set_process_input(true)

func _input(event):
	if event is InputEventMouseButton and event.pressed:
		if event.button_index == MOUSE_BUTTON_LEFT:
			if get_rect().has_point(get_local_mouse_position()):
				main_menu._on_start_button_pressed()
	
