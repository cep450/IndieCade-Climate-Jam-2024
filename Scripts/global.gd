extends Node

@onready var sim: Node = $"../Main/Simulation"
@export var inDevMode: bool = false
@export var audio_files: Array[Audio]
	
var current_tile: Vector2 = Vector2(-1, -1)

#Private
var audio_dict = {}
var stream_player = preload("res://Scenes/stream_player.tscn")

# Called by view_tile, Vector2 stores an index
func on_tile_clicked(clicked_tile: Vector2):
	if clicked_tile == current_tile:
		current_tile = Vector2(-1,-1)
	else: 
		current_tile = clicked_tile

func _ready():
	convert_files_to_dict()
	play("Wilkons")
	
func convert_files_to_dict():
	for file in audio_files:
		audio_dict[file.sound_name] = file.audio_stream

func play(sound_name: String):
	if (audio_dict.has(sound_name)):
		var stream = audio_dict[sound_name]
		var instance = stream_player.instantiate()
		add_child(instance)
		instance.stream = stream
		instance.audio_resource = stream
		instance.play()
		instance.track_finished.connect(on_finished)
	else:
		print("Could not find sound: " + sound_name)

func on_finished(player):
	if player.audio_resource.is_music():
		play("Wilkons")
	player.queue_free()
