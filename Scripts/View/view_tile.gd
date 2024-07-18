extends Node3D

# Public Variables
signal tile_clicked_signal(x: int, y: int)
var x: int
var y: int

# Private Variables
var UI = "/root/Main/UI"

# Block Scenes
var road = preload("res://Scenes/Tiles/Road1Straight.tscn")
var house = preload("res://Scenes/Tiles/BuildingGeneric01.tscn")
var base = preload("res://Scenes/Tiles/base.tscn")
var blank = preload("res://Scenes/Tiles/Blank.tscn")

var highlight_mat = preload("res://Resources/highlight_mat_overlay.tres")
var isYellow: bool = false

@onready var sim: Node = Global.sim

func test_init(type: String):
	var instance
	# Setup base or road.
	if type == "Road":
		instance = road.instantiate()
		add_child(instance)
		instance.name = "ObjectInstance"
	elif type == "Blank":
		instance = blank.instantiate()
		add_child(instance)
	else:
		instance = base.instantiate()
		add_child(instance)
		instance.name = "Base"
		
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
	get_parent().tile_clicked.connect(on_tile_clicked)
	
		
func select() -> void:
	get_parent().tile_clicked.emit(x,y)

func on_tile_clicked(local_x: int, local_y: int):
	
	# TODO the game crashes here because $ObjectInstance seems to be null.
	# errors print view_tile.gd:61 @ on_tile_clicked(): Node not found: "ObjectInstance" (relative to "/root/Main/View/World/@Node3D@196").
	# this if statement is just to keep the game from crashing for now.
	if($ObjectInstance == null):
		return
	
	if x == local_x && y == local_y:
		# Change the material to yellow_mat when selected
		if !isYellow:
			$ObjectInstance.get_child(0).material_overlay = highlight_mat
			isYellow = true
		else:
			$ObjectInstance.get_child(0).material_overlay = null
			isYellow = false
		update_visuals()
	else:
		$ObjectInstance.get_child(0).material_overlay = null
		isYellow = false

func update_visuals():
	# Clear existing children.
	for child in get_children():
		child.queue_free()
		
	# In case sime wasn't set for some reason.
	if(sim == null):
		sim = get_parent().sim
		
	# Generate new childreni
	#var infra = sim.GetInfra(x,y)
	#var instance
	#for type in infra:
		#if type.ModelHasBase:
			#instance = base.instantiate()
			#add_child(instance)
		#var model_path = type.path + get_version() + ".tscn"
		#instance = load(model_path).instantiate()
		#add_child(instance)
		#instance.rotation.y = get_new_rotation()	
		
func get_version() -> String:
	return ""

func get_new_rotation() -> float:
	return 0.0