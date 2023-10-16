extends CharacterBody2D

var playerSpeed
var tileMap

var moveButtonsParent
var cellIndicators

var fromPosition = Vector2(0,0)
var nextPosition = Vector2(0,0)
var positionDifference = Vector2(0,0)
var movement = 1.0;

func _ready():
	playerSpeed = get_node("/root/GameData").get("playerSpeed")
	tileMap = get_node("/root/Level/World/TileMap")
	moveButtonsParent = get_node("MoveControl")
	var spawnPoint = get_node("/root/GameData").get("spawnPoint")
	
	position = tileMap.map_to_local(spawnPoint)
	fromPosition = position
	nextPosition = position
	
	tileMap.on_enter_cell(tileMap.local_to_map(position))
	pass

func _process(delta):
	
	if (movement < 1):
		movement += delta * playerSpeed
		movement = minf(movement,1.0)
		position = fromPosition.lerp(nextPosition, movement)
	else:
		if (movement != 2.0):
			movement = 2.0
			moveButtonsParent.show()
			tileMap.on_enter_cell(tileMap.local_to_map(position))
	pass

func start_move(direction: int = 0):
	if (movement < 1):
		return
	
	moveButtonsParent.hide()
	
	fromPosition = position
	nextPosition = tileMap.get_neighbor(tileMap.local_to_map(position), direction)
	
	positionDifference = nextPosition - fromPosition
	
	movement = 0.0
	pass
