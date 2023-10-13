extends CharacterBody2D

var playerSpeed
var tileMap

var moveButtons
var currentCellLabel

var fromPosition = Vector2(0,0)
var nextPosition = Vector2(0,0)
var movement = 1.0;

func _ready():
	playerSpeed = get_node("/root/GameData").get("playerSpeed")
	tileMap = get_node("/root/Level/World/TileMap")
	moveButtons = get_node("MoveControl").get_children()
	currentCellLabel = get_node("OtherUI/CurrentCellLabel")
	var spawnPoint = get_node("/root/GameData").get("spawnPoint")
	
	position = tileMap.GetGlobalPos(spawnPoint) + Vector2(0,16)
	fromPosition = position
	nextPosition = position
	UpdateCellIndicators()
	pass

func _process(delta):
	
	if (movement < 1):
		movement += delta * playerSpeed
		movement = minf(movement,1.0)
		position = fromPosition.lerp(nextPosition, movement)
	else:
		if (movement != 2.0):
			movement = 2.0
			moveButtons[0].get_parent().show()
			tileMap.OnEnterCell(position + Vector2(32,32))
	pass

func StartMove(direction: int = 0):
	if (movement < 1):
		return
	
	moveButtons[0].get_parent().hide()
	
	fromPosition = position
	nextPosition = tileMap.GetNeighbor(position, direction)
	nextPosition.y += 16
	
	UpdateCellIndicators()
	
	movement = 0.0
	pass

func UpdateCellIndicators():
	var currentCell = tileMap.GetCellCoords(nextPosition + Vector2(32,32))
	currentCellLabel.text = str(tileMap.CalculateIndicatorForCell(currentCell, false))
	
	for button in moveButtons:
		var buttonCell = tileMap.GetNeighbor(nextPosition + Vector2(32,32), button.direction, false)
		var indicatorValue = tileMap.CalculateIndicatorForCell(buttonCell)
		button.SetText(str(indicatorValue))
	pass
