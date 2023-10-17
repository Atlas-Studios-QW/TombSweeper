extends Node

@export var mapSize = Vector2i(20,20)
@export var difficulty = Difficulty.Easy

@export var spawnPoint = Vector2i(5,5)
@export var playerSpeed = 2.0

@export var neighborDirectionIds = [14, 0, 2, 6, 8, 10]

@export var itemCoords = {}
@export var bombCoords = []
@export var exploredCoords = []

@export var cellLabels = {}
@export var cellLabelsParent: Control

func _ready():
	cellLabelsParent = get_node("/root/Level/Player/CellLabels")
	print(cellLabelsParent)
	pass

#difficulty is in %
enum Difficulty {
	Easy = 20,
	Normal = 30,
	Hard = 40
}

var roomTiles = {
	"Normal": Vector2i(0,0),
	"Bomb": Vector2i(0,1)
}

var itemTiles = {
	"Key": Vector2i(0,0),
	"Coin": Vector2i(1,0)
}
