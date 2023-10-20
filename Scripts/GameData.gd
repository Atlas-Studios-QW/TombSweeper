extends Node

@onready var overlayHandler = get_node("/root/Overlay")

@onready var saveData = SaveData.new()

var playerSpeed = 4.0

var neighborDirectionIds = [14, 0, 2, 6, 8, 10]
var canMove = true

class SaveData:
	var mapSize = Vector2i(10,10)
	var difficulty = Difficulty.Normal
	
	var itemCoords = {}
	var flagCoords = []
	var bombCoords = []
	var exploredCoords = []
	
	var cellLabels = {}
	
	var collectedItems = {
		"Key": 0,
		"Coin": 0
	}
	
	var tools = {
		"Radar" = Tool.new("Zooms out your view for 5 seconds", 5, 5, null),
		"Detonator" = Tool.new("When activated, you can select one cell around you to explode a bomb", 10, 0, null)
	}
	
	func _init():
		pass
	pass

func _get_tool(toolName: String):
	saveData.tools[toolName].collected = true
	overlayHandler._setup_tool(toolName, load("res://Sprites/Items/" + toolName + ".png"))
	pass

#difficulty is in %
enum Difficulty {
	Easy = 20,
	Normal = 30,
	Hard = 40
}

var roomTiles = {
	"Explored": Vector2i(0,0),
	"Unexplored": Vector2i(0,1),
	"NonEnterable": Vector2i(1,0)
}

var itemTiles = {
	"Key": Vector2i(0,0),
	"Coin": Vector2i(1,0),
	"Flag": Vector2i(0,1)
}

class Tool:
	var description: String
	var collected: bool
	var availability: int
	var requiredAvailability: int
	var effectDuration: int
	var button: Control
	
	func _init(describe: String, availabilityToUse: int, effectTime: int, UIButton: Control):
		description = describe
		collected = false
		availability = 0
		requiredAvailability = availabilityToUse
		effectDuration = effectTime
		button = UIButton
		pass
	pass
