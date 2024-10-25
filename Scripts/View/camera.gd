extends Camera3D

@export var PAN_SPEED: float
@export var ZOOM_SPEED: float
@export var ROTATION_SPEED: float
## Used to prevent camera from getting to close to scene.
@export var MIN_CAM_Y_POS: float

func _process(delta):
	# WASD to Pan
	var move_vector = Vector2.ZERO
		
	# Vector2 used to denote X,Z change vec.x = z, vec.y = x
			
	if Input.is_action_pressed("MoveViewUp"):
		move_vector += Vector2.RIGHT
	if Input.is_action_pressed("MoveViewDown"):
		move_vector += Vector2.LEFT
	if Input.is_action_pressed("MoveViewLeft"):
		move_vector += Vector2.DOWN
	if Input.is_action_pressed("MoveViewRight"):
		move_vector += Vector2.UP

	if(move_vector != Vector2.ZERO):	
		position.x -= (move_vector.rotated(rotation.y).y * PAN_SPEED * delta)
		position.z -= (move_vector.rotated(rotation.y).x * PAN_SPEED * delta)

	

func _unhandled_input(event):

	if event is InputEventMouseMotion && Input.is_mouse_button_pressed(MOUSE_BUTTON_RIGHT):

		# rotate the camera relative to itself, not the world 
		rotate_y(event.relative.x * ROTATION_SPEED * get_process_delta_time() / 10.0)

		#$"../World".rotate_y(event.relative.x * ROTATION_SPEED * get_process_delta_time() / 10.0)
		# Code for panning vai mouse move
		#position.x -= event.relative.x * PAN_SPEED * get_process_delta_time()
		#position.z -= event.relative.y * PAN_SPEED * get_process_delta_time()
		
	# Handle zoom.
	if event is InputEventMouseButton:
		# Check for scroll wheel.
		var direction = 0
		if event.button_index == MOUSE_BUTTON_WHEEL_UP:
			# Zoom in
			if (position.y > MIN_CAM_Y_POS):
				direction = 1
		elif event.button_index == MOUSE_BUTTON_WHEEL_DOWN:
			# Zoom out
			direction = -1
		if direction != 0:	
			var cam_forward = -transform.basis.z.normalized()
			position += direction * ZOOM_SPEED * cam_forward * event.get_factor() * get_process_delta_time()
	
		# Check for selection
		if event.button_index == MOUSE_BUTTON_LEFT && event.pressed:
			get_selection()
			
func get_selection():
	var worldspace = get_world_3d().direct_space_state
	var mouse_pos = get_viewport().get_mouse_position()
	var ray_length = 1000
	var start = project_ray_origin(mouse_pos)
	var end = start + project_ray_normal(mouse_pos) * ray_length
	var ray_query = PhysicsRayQueryParameters3D.new()
	ray_query.from = start
	ray_query.to = end
	var result = worldspace.intersect_ray(ray_query)
	if !result.is_empty():
		var tile = result.collider.get_parent()
		if tile.has_method("select"):
			tile.select()
		
