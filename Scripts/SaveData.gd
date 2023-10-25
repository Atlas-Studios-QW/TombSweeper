extends Resource
class_name SavegameData

@export var mapSize: Vector2i
@export var difficulty: Difficulty

@export var playerPosition: Vector2

@export var itemCoords = {}
@export var flagCoords = []
@export var bombCoords = []
@export var exploredCoords = []

@export var collectedTools = {}
@export var collectedItems = {
	"Key": 0,
	"Coin": 0
}

func _init(newMapSize:= Vector2i(0,0), newDifficulty:= Difficulty.Normal):
	mapSize = newMapSize
	difficulty = newDifficulty
	pass

enum Difficulty {
	Easy = 20,
	Normal = 30,
	Hard = 40
}
