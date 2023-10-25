extends Node

@onready var overlayHandler = get_node("/root/Overlay")

var loadSave: bool

var saveData: SavegameData
var localSaveData: SavegameData

var playerSpeed = 4.0

var neighborDirectionIds = [14, 0, 2, 6, 8, 10]
var canMove = true

func save_progress():
	if (ResourceLoader.exists("user://savegame.tres")):
		localSaveData = ResourceLoader.load("user://savegame.tres")
	else:
		localSaveData = SavegameData.new()
	
	localSaveData = saveData
	
	var result = ResourceSaver.save(localSaveData, "user://savegame.tres")
	print("Save result:" + str(result))
	pass

func load_progress():
	saveData = SavegameData.new()
	saveData = ResourceLoader.load("user://savegame.tres") as SavegameData
	print("Loaded savedata: " + str(saveData))
	pass

func _get_tool(toolName, availability := 0):
	saveData.collectedTools[toolName] = availability
	overlayHandler._setup_tool(toolName, load("res://Sprites/Tools/" + toolName + ".png"))
	pass

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

var tools = {
	"Radar" = Tool.new("Zooms out your view for 5 seconds", 5, 5, null),
	"Detonator" = Tool.new("When activated, you can select one cell around you to explode a bomb", 10, 0, null)
}

class Tool:
	var description: String
	var requiredAvailability: int
	var effectDuration: int
	var button: Control
	
	func _init(describe: String, availabilityToUse: int, effectTime: int, UIButton: Control):
		description = describe
		requiredAvailability = availabilityToUse
		effectDuration = effectTime
		button = UIButton
		pass
	pass
