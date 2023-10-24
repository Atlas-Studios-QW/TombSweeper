extends Node

@onready var overlayHandler = get_node("/root/Overlay")

var loadSave: bool

var saveData: SaveData

var playerSpeed = 4.0

var neighborDirectionIds = [14, 0, 2, 6, 8, 10]
var canMove = true

func save_progress():
	var result = ResourceSaver.save(saveData, "user://savegame.tres")
	print("Save result:" + str(result))
	pass

func load_progress():
	saveData = SaveData.new(Vector2i(10,10), SaveData.Difficulty.Normal)
	var data = ResourceLoader.load("user://savegame.tres", "SaveData").duplicate(true)
	print("Loaded: " + str(data))
	saveData = data as SaveData
	pass

func _get_tool(toolName: String):
	saveData.tools[toolName].collected = true
	overlayHandler._setup_tool(toolName, load("res://Sprites/Items/" + toolName + ".png"))
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
