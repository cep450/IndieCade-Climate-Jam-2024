extends Node

@onready var sim: Node = $"../Main/Simulation"
@export var inDevMode: bool = false
@export var audio_files: Array[Audio]
	
var current_tile: Vector2 = Vector2(-1, -1)

#Private audio stuff
var audio_dict = {}
var stream_player = preload("res://Scenes/stream_player.tscn")
var music_tracks: Array[String] =  ["OST-A","OST-B"]
var music_index: int = 0
# Called by view_tile, Vector2 stores an index
func on_tile_clicked(clicked_tile: Vector2):
	if clicked_tile == current_tile:
		current_tile = Vector2(-1,-1)
	else: 
		current_tile = clicked_tile

func _ready():
	convert_files_to_dict()
	play(music_tracks[music_index])
	
func convert_files_to_dict():
	for file in audio_files:
		audio_dict[file.sound_name] = file

func play(sound_name: String):
	if (audio_dict.has(sound_name)):
		var audio_res = audio_dict[sound_name]
		var instance = stream_player.instantiate()
		add_child(instance)
		instance.stream = audio_res.audio_stream
		instance.audio_res = audio_res
		instance.play()
		instance.name = sound_name
		instance.track_finished.connect(on_finished)
	else:
		print("Could not find sound: " + sound_name)

func on_finished(player):
	if player.audio_res.is_music:
		incriment_music_index()
		play(music_tracks[music_index])
	player.queue_free()

func _input(event):
	if event is InputEventKey:
		if event.pressed and event.keycode == KEY_M:
			for child in get_children():
				if child.audio_res.is_music:
					child.queue_free()
					incriment_music_index()
					play(music_tracks[music_index])
						

# Helpers
func incriment_music_index():
	music_index = (music_index + 1) % music_tracks.size()
