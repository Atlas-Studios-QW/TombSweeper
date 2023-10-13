extends CharacterBody2D

var playerSpeed
var tileMap

var moveButtons

var fromPosition = Vector2(0,0)
var nextPosition = Vector2(0,0)
var movement = 1.0;

func _ready():
	playerSpeed = get_node("/root/GameData").get("playerSpeed")
	tileMap = get_node("/root/Level/World/TileMap")
	moveButtons = get_node("MoveControl")
	var spawnPoint = get_node("/root/GameData").get("spawnPoint")
	
	position = tileMap.GetGlobalPos(spawnPoint)
	pass

func _process(delta):
	
	if (movement < 1):
		movement += delta * playerSpeed
		movement = minf(movement,1.0)
		position = fromPosition.lerp(nextPosition, movement)
	else:
		moveButtons.show()
	pass

func StartMove(direction: int = 0):
	if (movement < 1):
		return
	
	moveButtons.hide()
	
	fromPosition = position
	nextPosition = tileMap.GetGlobalNeighbor(position, direction)
	
	#Offset to center player
	nextPosition.y += 16
	
	movement = 0.0
	pass
