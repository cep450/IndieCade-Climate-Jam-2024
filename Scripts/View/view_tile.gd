extends Node3D

# Public Variables
signal tile_clicked_signal(x: int, y: int)
var x: int
var y: int

# Block Scenes
var base = preload("res://Scenes/Tiles/Base.tscn")
var blank = preload("res://Scenes/Tiles/Blank.tscn")
var road_resource: SimInfraType = preload("res://Resources/InfraTypes/road.tres")

var highlight_mat = preload("res://Resources/highlight_mat_overlay.tres")
var isYellow: bool = false
var model_connects: bool = false
var variant: String = ""

@onready var sim: Node = Global.sim

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
	# Don't re-update visuals for non connecting models
	if repeated && !model_connects:
		return
	# Clear existing children.
	for child in get_children():
		child.queue_free()
		
	# In case sime wasn't set for some reason.
	if(sim == null):     
		sim = Global.sim
		
	# Generate new children
	var infra = sim.GetInfra(x,y)
	var instance
	model_connects = false
	if infra.is_empty() && Global.inDevMode:
		instance = blank.instantiate()
		add_child(instance)
	else:
		for type in infra:
			if type.ModelHasBase:
				instance = base.instantiate()
				add_child(instance)
			if type.ModelConnects:
				model_connects = true
			if !type.ModelPath.is_empty():
				print(type.Name)
				# Note that 'get_version() also rotates as needed.
				var full_path = type.ModelPath + get_variant(type) + get_version(type) + ".tscn"
				var model = load(full_path)
				instance = model.instantiate()
				add_child(instance)
				if !repeated:
					get_parent().update_neighbors(Vector2i(x,y))
			else: 
				print("path not given")	
		
func get_variant(type) -> String:
	if type.ModelVariantCount < 2:
		return ""
	if variant != "":
		return variant
	var random = RandomNumberGenerator.new()
	var variant_string = str(random.randi_range(1, type.ModelVariantCount))
	variant = variant_string
	return variant_string

func get_version(type: SimInfraType) -> String:
	if !type.ModelConnects:
		return ""
	var versionInfo = sim.grid.GetVersion(Vector2i(x,y),road_resource)
	# Only use road to determine rotation.
	rotation = versionInfo.rotation
	return versionInfo.versionString

func update_highlight():
	for child in get_children():
		if isYellow:
			child.get_child(0).material_overlay = highlight_mat
		else: 
			child.get_child(0).material_overlay = null
