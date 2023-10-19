extends CanvasLayer

@onready var gameData = get_node("/root/GameData")

@onready var animator = get_node("OverlayAnimator")

@onready var toolButtonPrefab = preload("res://Prefabs/UI/ToolButton.tscn")

@export var coinsCounter: Label
@export var toolsParent: Node

func _update_from_gamedata():
	_update_coin_counter(gameData.collectedItems["Coin"])
	
	for toolName in gameData.tools:
		var tool: GameData.Tool = gameData.tools[toolName]
		if (tool.button != null):
			_update_tool_button(toolName, tool.availability)
	pass

func _update_after_move(unexploredCell: bool):
	if (unexploredCell):
		for toolName in gameData.tools:
			gameData.tools[toolName].availability += 1
	
	_update_from_gamedata()
	pass

func _update_coin_counter(newAmount: int):
	var newText = str(newAmount)
	if (newText != coinsCounter.text):
		animator.play("CoinCollected")
	coinsCounter.text = newText
	pass

func _setup_tool(toolName: String, toolIcon: Texture):
	if (gameData.tools[toolName].button != null):
		return
	
	toolsParent.add_child(toolButtonPrefab.instantiate())
	var newToolButton = toolsParent.get_child(toolsParent.get_child_count() - 1)
	newToolButton.get_node("ToolName").text = toolName
	newToolButton.get_node("ToolIcon").texture = toolIcon
	newToolButton.get_node("Button").connect("button_up", Callable(get_node("/root/Level/Player/Body"), "use_tool").bind(toolName))
	gameData.tools[toolName].button = newToolButton
	pass

func _update_tool_button(toolName: String, newProgress: float):
	gameData.tools[toolName].button.get_node("Availability").value = newProgress
	pass
