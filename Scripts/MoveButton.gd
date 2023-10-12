extends Button

var movementController
@export var direction: int = 0

func _ready():
	movementController = get_node("../../")
	pass

func _pressed():
	movementController.StartMove(direction)
	pass
