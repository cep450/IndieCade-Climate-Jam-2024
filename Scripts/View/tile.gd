extends StaticBody3D


var road = preload("res://Scenes/Tiles/tile_road_1_straight.tscn")
var base = preload("res://Models/Tile_Base.glb")
var house = preload("res://Scenes/Tiles/tile_house_1.tscn")
var work = preload("res://Models/Tile_Building1.glb")

# Private Variables
var highlight_mat = preload("res://Resources/highlight_mat_overlay.tres")
var grid
var isYellow: bool

func _ready() -> void:
	isYellow = false

func test_init(type: String):
	var instance
	if type == "Road":
		instance = road.instantiate()
	if type == "House":
		instance = house.instantiate()
	add_child(instance)
	instance.name = "ObjectInstance"
	if type != "Road":
		instance = base.instantiate()
		add_child(instance)

func initialize(_x: int, _y: int) -> void:
	# Access grid and get info from there or maybe call directly from Sim.cs?
	#grid = $"../../Simulation/SimGrid"
	pass
		
func select() -> void:
	# Change the material to yellow_mat when selected
	if isYellow:
		$ObjectInstance.get_child(0).material_overlay = null
	else:
		$ObjectInstance.get_child(0).material_overlay = highlight_mat
	#$MeshInstance3D.mesh.surface_set_material(0, mat)
	isYellow = !isYellow
