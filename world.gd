extends Node3D

var agent = preload("res://agent.tscn")
var res = []
var Simulation 
func _ready(): 
	var instance = agent.instantiate()
	add_child(instance)
	Simulation = get_node("/root/World/Sim") 
	print(Simulation.TestPrint())
	var list = Simulation.GetAgents()
	for item in list:
		print(item)

func findByClass(node: Node, className : String, result : Array) -> void:
	if node.is_class(className) :
		result.push_back(node)
		print("Found")
		for child in node.get_children():
			findByClass(child, className, result)
