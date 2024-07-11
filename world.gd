extends Node3D

var agent = preload("res://agent.tscn")
var res = []
var Simulation 
var meshList = []
func _ready(): 
	## Gets the main sim node to find the agent list, should make it update every time one is added
	Simulation = get_node("/root/World/Sim") 
	print(Simulation.TestPrint())
	var agentList = Simulation.GetAgents()
	## Every agent gets a mesh added to the scene
	for item in agentList:
		var agent = agent.instantiate()
		## Saved in a 2d array where each entry holds the mesh and its corresponding script agent
		meshList.push_back([agent, item])
		agent.position = Vector3(item.currentPosition.x, 0, item.currentPosition.y)
		add_child(agent)
		print(item)
		
func _process(_delta):
	for meshAgent in meshList:
		meshAgent[0].position = Vector3(meshAgent[1].Vehicle.CurrentPosition.x, 0, meshAgent[1].Vehicle.CurrentPosition.y)
	

func findByClass(node: Node, className : String, result : Array) -> void:
	if node.is_class(className) :
		result.push_back(node)
		print("Found")
		for child in node.get_children():
			findByClass(child, className, result)
