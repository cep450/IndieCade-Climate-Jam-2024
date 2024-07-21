extends AudioStreamPlayer2D

signal track_finished(instance)
var audio_resource: Audio

func _on_finished():
	track_finished.emit(self)
