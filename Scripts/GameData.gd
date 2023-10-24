extends Node

@onready var overlayHandler = get_node("/root/Overlay")

var loadSave: bool

var saveData: SaveData

var playerSpeed = 4.0

var neighborDirectionIds = [14, 0, 2, 6, 8, 10]
var canMove = true

func save_progress():
	var result = ResourceSaver.save(saveData, "user://savegame.res")
	print("Save result:" + str(result))
	pass

func load_progress():
	var data = load("user://savegame.res")
	print("Loaded: " + str(data))
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
