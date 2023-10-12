extends CharacterBody2D

var playerSpeed
var tileMap

var nextPosition = Vector2(0,0)
var isMoving = false;

func _ready():
	playerSpeed = get_node("/root/GameData").get("playerSpeed")
	tileMap = get_node("../../World/TileMap")
	pass

func _process(delta):
	isMoving = position != nextPosition
	
	if (isMoving):
		var positionDifference = nextPosition - position
		var vector = positionDifference * delta * playerSpeed
		position += vector
	pass

func StartMove(direction: int = 0):
	if (isMoving):
		pass
	
	var nextPosition = tileMap.GetGlobalNeighbor(position, direction)
	pass
