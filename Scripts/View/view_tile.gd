extends Node3D

# Public Variables
signal tile_clicked_signal(x: int, y: int)
var x: int
var y: int

# Private Variables
var UI = "/root/Main/UI"

# Block Scenes
var road = preload("res://Scenes/Tiles/road.tscn")
var base = preload("res://Scenes/Tiles/base.tscn")
var house = preload("res://Scenes/Tiles/BuildingGeneric01.tscn")
var work = preload("res://Models/Tile_Building1.glb")

var highlight_mat = preload("res://Resources/highlight_mat_overlay.tres")
var isYellow: bool = false

var grid

func _ready() -> void:
	pass

func test_init(type: String):
	var instance
	# Setup base or road.
	if type == "Road":
		instance = road.instantiate()
		add_child(instance)
		instance.name = "ObjectInstance"
	else:
		instance = base.instantiate()
		add_child(instance)
		
	#If not road put object ontop of base.
	if type == "House":
		instance = house.instantiate()
		add_child(instance)
	instance.name = "ObjectInstance"

func initialize(local_x: int, local_y: int) -> void:
	# Access grid and get info from there or maybe call directly from Sim.cs?
	#grid = $"../../Simulation/SimGrid"
	x = local_x
	y = local_y
	get_parent().tile_clicked.connect(_on_tile_clicked)
	
		
func select() -> void:
	get_parent().tile_clicked.emit(x,y)

func _on_tile_clicked(local_x: int, local_y: int):
	if x == local_x && y == local_y:
		# Change the material to yellow_mat when selected
		if !isYellow:
			$ObjectInstance.get_child(0).material_overlay = highlight_mat
			isYellow = true
		else:
			$ObjectInstance.get_child(0).material_overlay = null
			isYellow = false
	else:
		$ObjectInstance.get_child(0).material_overlay = null
		isYellow = false
