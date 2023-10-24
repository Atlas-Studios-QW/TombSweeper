extends Resource

class_name SaveData

@export var mapSize: Vector2i
@export var difficulty: Difficulty

@export var itemCoords = {}
@export var flagCoords = []
@export var bombCoords = []
@export var exploredCoords = []

@export var cellLabels = {}

@export var collectedItems = {
	"Key": 0,
	"Coin": 0
}

@export var tools = {
	"Radar" = Tool.new("Zooms out your view for 5 seconds", 5, 5, null),
	"Detonator" = Tool.new("When activated, you can select one cell around you to explode a bomb", 10, 0, null)
}

func _init(newMapSize: Vector2i, newDifficulty: Difficulty):
	mapSize = newMapSize
	difficulty = newDifficulty
	pass

enum Difficulty {
	Easy = 20,
	Normal = 30,
	Hard = 40
}

class Tool:
	var description: String
	var collected: bool
	var availability: int
	var requiredAvailability: int
	var effectDuration: int
	var button: Control
	
	func _init(describe: String, availabilityToUse: int, effectTime: int, UIButton: Control):
		description = describe
		collected = false
		availability = 0
		requiredAvailability = availabilityToUse
		effectDuration = effectTime
		button = UIButton
		pass
	pass
