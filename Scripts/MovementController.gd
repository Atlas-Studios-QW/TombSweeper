extends CharacterBody2D

@onready var overlayHandler = get_node("/root/Overlay")
@onready var gameData = get_node("/root/GameData")

@onready var animator: AnimationPlayer = get_node("../PlayerAnimator")
@onready var playerSpeed = gameData.get("playerSpeed")
@onready var tileMap: TileMap = get_node("/root/Level/World/RoomsMap")
@onready var moveButtonsParent = get_node("MoveControl")
@onready var camera: Camera2D = get_node("Camera")

var fromPosition = Vector2(0,0)
var nextPosition = Vector2(0,0)
var positionDifference = Vector2(0,0)
var movement = 1.0;

@onready var timer = Timer.new()

func _ready():
	var spawnPoint = get_node("/root/GameData").get("spawnPoint")
	position = tileMap.map_to_local(spawnPoint)
	
	fromPosition = position
	nextPosition = position
	
	tileMap.on_enter_cell(tileMap.local_to_map(position))
	
	gameData._get_tool("Radar")
	pass

func _process(delta):
	if (movement < 1):
		movement += delta * playerSpeed
		movement = minf(movement,1.0)
		position = fromPosition.lerp(nextPosition, movement)
	else:
		if (movement != 2.0):
			movement = 2.0
			var newCell = !tileMap.exploredCoords.has(tileMap.local_to_map(position))
			var playerWon = tileMap.on_enter_cell(tileMap.local_to_map(position))
			moveButtonsParent.show()
			overlayHandler._update_after_move(newCell)
	pass

func _input(event):
	var mouseDistance = get_global_mouse_position().distance_to(position)
	if (mouseDistance > 100):
		return
	
	var mouseDirection = get_global_mouse_position().angle_to_point(position)
	var direction = roundf(rad_to_deg(mouseDirection) / 60 + 4)
	
	if (direction > 5):
		direction = fmod(direction, 6.0)

	if (event is InputEventMouseButton and event.pressed):
		match event.button_index:
			MOUSE_BUTTON_LEFT:
				start_move(direction)
			MOUSE_BUTTON_RIGHT:
				set_flag(direction)
	pass

func start_move(direction: int):
	if (movement < 1):
		return
	
	var nextCoords = tileMap.get_neighbor(tileMap.local_to_map(position), direction)
	
	if (!check_valid_move(nextCoords)):
		return
	
	moveButtonsParent.hide()
	
	fromPosition = position
	nextPosition = tileMap.map_to_local(nextCoords)
	movement = 0.0
	pass

func set_flag(direction: int):
	var nextCoords = tileMap.get_neighbor(tileMap.local_to_map(position), direction)
	tileMap.set_flag(nextCoords)
	pass

func check_valid_move(coords: Vector2i):
	if (!tileMap.check_bounds(coords)):
		return false
	if (tileMap.flagCoords.has(coords)):
		return false
	return true

func use_tool(toolName: String):
	var toolData: GameData.Tool = gameData.tools[toolName]
	match toolName:
		"Radar":
			if (toolData.availability >= toolData.requiredAvailability):
				toolData.availability = 0
				animator.play("RadarZoom")
	
	overlayHandler._update_from_gamedata()
	pass
