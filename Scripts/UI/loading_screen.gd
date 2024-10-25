extends RichTextLabel

var markup = "[center]"
@onready var background = $Background

var began_loading = false
var loading = false
var load_progress : Array[float]
var scene_loading_path

func _ready():
	background.visible = false

func request_load_scene(scene_path):
	visible = true
	background.visible = true
	scene_loading_path = scene_path
	set_loading_text("LOADING MAP...")
	loading = true

func load_scene():
	ResourceLoader.load_threaded_request(scene_loading_path)
	began_loading = true

func set_loading_text(new_text: String):
	set_text(markup + new_text)
	background.visible = true

func _process(_delta: float):

	if(!loading): return 

	if(loading && !began_loading):
		load_scene()
		return
	
	var loading_status = ResourceLoader.load_threaded_get_status(scene_loading_path, load_progress)

	match loading_status:
		ResourceLoader.THREAD_LOAD_IN_PROGRESS:
			set_loading_text("LOADING MAP... " + str(load_progress[0] * 100) + "%")
		ResourceLoader.THREAD_LOAD_LOADED:
			# finished loading
			get_tree().change_scene_to_packed(ResourceLoader.load_threaded_get(scene_loading_path))
		ResourceLoader.THREAD_LOAD_FAILED:
			print("ERR: could not load")
