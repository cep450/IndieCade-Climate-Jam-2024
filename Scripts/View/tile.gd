extends StaticBody3D

var blue_mat = preload("res://Resources/blue_mat.tres")
var yellow_mat = preload("res://Resources/yellow_mat.tres")

var isYellow: bool

func _ready():
	# Ensure the mesh has its own material instance by duplicating it
	$MeshInstance3D.set_mesh($MeshInstance3D.mesh.duplicate())
	$MeshInstance3D.mesh.surface_set_material(0, blue_mat)
	isYellow = false

func select():
	# Change the material to yellow_mat when selected
	var mat
	if isYellow:
		mat = blue_mat
	else:
		mat = yellow_mat
	$MeshInstance3D.mesh.surface_set_material(0, mat)
	isYellow = !isYellow
