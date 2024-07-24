extends AudioStreamPlayer2D

signal track_finished(instance)
var audio_res: Audio

func _on_finished():
	track_finished.emit(self)
