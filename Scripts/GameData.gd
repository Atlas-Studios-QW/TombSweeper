extends Node

@export var mapSize = Vector2i(10,10)
@export var difficulty = Difficulty.Hard

@export var spawnPoint = Vector2i(5,5)
@export var playerSpeed = 2.0

@export var neighborDirectionIds = [14, 0, 2, 6, 8, 10]

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
	Easy = 15,
	Normal = 25,
	Hard = 35
}
