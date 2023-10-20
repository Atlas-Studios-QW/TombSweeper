extends CanvasLayer

@onready var sceneManager = get_node("/root/SceneManager")
@onready var gameData = get_node("/root/GameData")
@onready var saveData: GameData.SaveData = gameData.saveData

@onready var animator = get_node("OverlayAnimator")

@onready var toolButtonPrefab = preload("res://Prefabs/UI/ToolButton.tscn")

@export_category("Nodes")
@export var pauseMenu: Node
@export var endScreen: Node
@export var coinsCounter: Label
@export var toolsParent: Node
@export var detonatorUI: Node

func overlay_visibility(setVisible: bool):
	if (setVisible):
		endScreen.visible = false
		pauseMenu.visible = false
		detonatorUI.visible = false
	self.visible = setVisible
	pass

func _ready():
	overlay_visibility(false)
	endScreen.get_node("ReturnButton").connect("button_up", Callable(self, "end_game"))
	pass

func end_game():
	overlay_visibility(false)
	sceneManager.load_scene("MainMenu")
	pass

func _update_from_gamedata():
	_update_coin_counter(saveData.collectedItems["Coin"])
	
	for toolName in saveData.tools:
		var tool: GameData.Tool = saveData.tools[toolName]
		if (tool.button != null):
			_update_tool_button(toolName, tool.availability)
	pass

func _update_after_move(unexploredCell: bool):
	if (unexploredCell):
		for toolName in saveData.tools:
			saveData.tools[toolName].availability += 1
	
	_update_from_gamedata()
	pass

func _update_coin_counter(newAmount: int):
	var newText = str(newAmount)
	if (newText != coinsCounter.text):
		animator.play("CoinCollected")
	coinsCounter.text = newText
	pass

func _setup_tool(toolName: String, toolIcon: Texture):
	if (saveData.tools[toolName].button != null):
		return
	
	toolsParent.add_child(toolButtonPrefab.instantiate())
	var newToolButton = toolsParent.get_child(toolsParent.get_child_count() - 1)
	newToolButton.get_node("ToolName").text = toolName
	newToolButton.get_node("ToolIcon").texture = toolIcon
	var availabilityFIll: TextureProgressBar = newToolButton.get_node("Availability")
	availabilityFIll.max_value = saveData.tools[toolName].requiredAvailability
	newToolButton.get_node("Button").connect("button_up", Callable(get_node("/root/Level/Player/Body"), "use_tool").bind(toolName))
	newToolButton.get_node("Button").connect("mouse_entered", Callable(self, "tooltip_visibility").bind(toolName, true))
	newToolButton.get_node("Button").connect("mouse_exited", Callable(self, "tooltip_visibility").bind(toolName, false))
	newToolButton.get_node("Tooltip/TooltipText").text = saveData.tools[toolName].description
	newToolButton.get_node("Tooltip/AvailableText").text = "Available every " + str(saveData.tools[toolName].requiredAvailability) + " new cells"
	saveData.tools[toolName].button = newToolButton
	pass

func _update_tool_button(toolName: String, newProgress: float):
	saveData.tools[toolName].button.get_node("Availability").value = newProgress
	pass

func show_game_result(gameWon: bool):
	gameData.canMove = false
	endScreen.visible = true
	var resultLabel = endScreen.get_node("GameResultText")
	if (gameWon):
		resultLabel.text = "You Won!"
	else:
		resultLabel.text = "You Lost"
	
	resultLabel.text += "\n\nTotal Coins:\n" + str(saveData.collectedItems["Coin"])
	pass

func tooltip_visibility(toolName: String, setVisible: bool):
	saveData.tools[toolName].button.get_node("Tooltip").visible = setVisible
	pass

func detonator_visibility(setVisible: bool):
	detonatorUI.visible = setVisible
	pass
