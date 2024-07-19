extends Node3D

# Public Variables
signal tile_clicked_signal(x: int, y: int)
var x: int
var y: int

# Block Scenes
var base = preload("res://Scenes/Tiles/base.tscn")
var blank = preload("res://Scenes/Tiles/Grass.tscn")

var highlight_mat = preload("res://Resources/highlight_mat_overlay.tres")
var isYellow: bool = false

@onready var sim: Node = Global.sim

func test_init(type: String):
	if type == "Blank":
		var instance = blank.instantiate()
		add_child(instance)

func initialize(local_x: int, local_y: int) -> void:
	# Access grid and get info from there or maybe call directly from Sim.cs?
	#grid = $"../../Simulation/SimGrid"
	x = local_x
	y = local_y
	get_parent().tile_clicked.connect(on_tile_clicked)
	
		
func select() -> void:
	Global.on_tile_clicked(Vector2(x,y))
	get_parent().tile_clicked.emit(x,y)

func on_tile_clicked(local_x: int, local_y: int):
	if x == local_x && y == local_y:
		isYellow = !isYellow
	else:
		isYellow = false
	update_highlight()
	
func update_visuals(repeated: bool = false):
	# Clear existing children.
	for child in get_children():
		child.queue_free()
		
	# In case sime wasn't set for some reason.
	if(sim == null):
		sim = Global.sim
		
	# Generate new children
	var infra = sim.GetInfra(x,y)
	var instance
	if infra.is_empty():
		instance = blank.instantiate()
		add_child(instance)
	else:
		for type in infra:
			if type.ModelHasBase:
				instance = base.instantiate()
				add_child(instance)
			if !type.ModelPath.is_empty():
				# Note that 'get_version() also rotates as needed.
				var full_path = type.ModelPath + get_version(type) + ".tscn"
				var model = load(full_path)
				instance = model.instantiate()
				add_child(instance)
				# TODO for optimization, rewrite this call to only reupdate orthognally
				# adjacent tiles.
				if (!repeated):
					get_parent().update_all_tile_visuals()
			else: 
				print("path not given")


func get_version(type: SimInfraType) -> String:
	if !type.ModelConnects:
		return ""
	var versionInfo = sim.grid.GetVersion(Vector2i(x,y),type)
	var test_r = versionInfo.rotation
	rotation = versionInfo.rotation
	return versionInfo.versionString

func update_highlight():
	for child in get_children():
		if isYellow:
			child.get_child(0).material_overlay = highlight_mat
		else: 
			child.get_child(0).material_overlay = null
