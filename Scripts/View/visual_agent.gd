extends Node3D

func Set_Pos(pos: Vector3):
	position = pos;

func Set_Vehicle(path: String):
	for child in get_children():
		child.queue_free()
	if path != "":
		var instance = load(path).instantiate()
		add_child(instance)

func Set_Visible(is_visible_b: bool):
	visible = is_visible_b
"res://Scenes/Tiles/ParkingLotEmpty.tscns"
