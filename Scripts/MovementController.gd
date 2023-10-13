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
	moveButtons = get_node("MoveControl").get_children()
	var spawnPoint = get_node("/root/GameData").get("spawnPoint")
	
	position = tileMap.GetGlobalPos(spawnPoint)
	pass

func _process(delta):
	
	if (movement < 1):
		movement += delta * playerSpeed
		movement = minf(movement,1.0)
		position = fromPosition.lerp(nextPosition, movement)
	else:
		moveButtons[0].get_parent().show()
	pass

func StartMove(direction: int = 0):
	if (movement < 1):
		return
	
	moveButtons[0].get_parent().hide()
	
	nextPosition = tileMap.GetNeighbor(position, direction)
	
	for button in moveButtons:
		var buttonCell = tileMap.GetNeighbor(fromPosition, button.direction, false)
		var indicatorValue = tileMap.CalculateIndicatorForCell(buttonCell)
		button.SetText(str(indicatorValue))
	
	fromPosition = position
	nextPosition.y += 16
	
	movement = 0.0
	pass
