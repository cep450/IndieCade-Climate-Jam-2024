extends Camera3D

const PAN_SPEED = 1;
const ZOOM_SPEED = 20;

func _input(event):
	if event is InputEventMouseMotion && Input.is_mouse_button_pressed(MOUSE_BUTTON_RIGHT):
		position.x -= event.relative.x * PAN_SPEED * get_process_delta_time()
		position.z -= event.relative.y * PAN_SPEED * get_process_delta_time()
	# Handle zoom.
	if event is InputEventMouseButton:
		# Check for scroll wheel.
		var direction = 0
		if event.button_index == MOUSE_BUTTON_WHEEL_UP:
			# Zoom in
			direction = 1
		elif event.button_index == MOUSE_BUTTON_WHEEL_DOWN:
			# Zoom out
			direction = -1
		if direction != 0:	
			var cam_forward = -transform.basis.z.normalized()
			position -= direction * ZOOM_SPEED * cam_forward * event.get_factor() * get_process_delta_time()
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
		var tile = result.collider
		if tile.has_method("select"):
			tile.select()
		
