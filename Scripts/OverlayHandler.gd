extends CanvasLayer

@onready var sceneManager = get_node("/root/SceneManager")
@onready var gameData = get_node("/root/GameData")

@onready var animator = get_node("OverlayAnimator")

@onready var toolButtonPrefab = preload("res://Prefabs/UI/ToolButton.tscn")

@export_category("Nodes")
@export var pauseMenu: Node
@export var endScreen: Node
@export var coinsCounter: Label
@export var toolsParent: Node
@export var detonatorUI: Node

func overlay_visibility(setVisible: bool, resetOverlay := false):
	self.visible = setVisible
	if (resetOverlay):
		var tools = toolsParent.get_children()
		for toolButton in tools:
			toolsParent.remove_child(toolButton)
		endScreen.visible = false
		pauseMenu.visible = false
		detonatorUI.visible = false
		for tool in gameData.tools:
			gameData.tools[tool].button = null
	pass

func _ready():
	overlay_visibility(false)
	endScreen.get_node("ReturnButton").connect("button_up", Callable(self, "end_game"))
	pauseMenu.get_node("MainMenu").connect("button_up", Callable(self, "end_game"))
	pauseMenu.get_node("Save").connect("button_up", Callable(gameData, "save_progress"))
	pass

func _unhandled_key_input(_event):
	if Input.is_key_pressed(KEY_ESCAPE):
		toggle_pause_menu()
	pass

func toggle_pause_menu():
	var isVisible = pauseMenu.visible
	pauseMenu.visible = !isVisible
	gameData.canMove = isVisible
	pass

func end_game():
	overlay_visibility(false, true)
	sceneManager.load_scene("MainMenu")
	pass

func _update_from_gamedata():
	_update_coin_counter(gameData.saveData.collectedItems["Coin"])
	
	for toolName in gameData.saveData.collectedTools:
		var tool : GameData.Tool = gameData.tools[toolName]
		if (tool.button != null):
			_update_tool_button(toolName, gameData.saveData.collectedTools[toolName])
	pass

func _update_after_move(unexploredCell: bool):
	if (unexploredCell):
		for toolName in gameData.saveData.collectedTools:
			gameData.saveData.collectedTools[toolName] += 1
	_update_from_gamedata()
	pass

func _update_coin_counter(newAmount: int):
	if (newAmount > (coinsCounter.text as int)):
		animator.play("CoinCollected")
	coinsCounter.text = str(newAmount)
	pass

func _setup_tool(toolName: String, toolIcon: Texture):
	if (gameData.tools[toolName].button != null):
		return
	
	toolsParent.add_child(toolButtonPrefab.instantiate())
	var newToolButton = toolsParent.get_child(toolsParent.get_child_count() - 1)
	newToolButton.get_node("ToolName").text = toolName
	newToolButton.get_node("ToolIcon").texture = toolIcon
	var availabilityFIll: TextureProgressBar = newToolButton.get_node("Availability")
	availabilityFIll.max_value = gameData.tools[toolName].requiredAvailability
	newToolButton.get_node("Button").connect("button_up", Callable(get_node("/root/Level/Player/Body"), "use_tool").bind(toolName))
	newToolButton.get_node("Button").connect("mouse_entered", Callable(self, "tooltip_visibility").bind(toolName, true))
	newToolButton.get_node("Button").connect("mouse_exited", Callable(self, "tooltip_visibility").bind(toolName, false))
	newToolButton.get_node("Tooltip/TooltipText").text = gameData.tools[toolName].description
	newToolButton.get_node("Tooltip/AvailableText").text = "Available every " + str(gameData.tools[toolName].requiredAvailability) + " new cells"
	gameData.tools[toolName].button = newToolButton
	pass

func _update_tool_button(toolName: String, newProgress: float):
	gameData.tools[toolName].button.get_node("Availability").value = newProgress
	pass

func show_game_result(gameWon: bool):
	gameData.canMove = false
	endScreen.visible = true
	var resultLabel = endScreen.get_node("GameResultText")
	if (gameWon):
		resultLabel.text = "You Won!"
	else:
		resultLabel.text = "You Lost"
	
	resultLabel.text += "\n\nTotal Coins:\n" + str(gameData.saveData.collectedItems["Coin"])
	pass

func tooltip_visibility(toolName: String, setVisible: bool):
	gameData.tools[toolName].button.get_node("Tooltip").visible = setVisible
	pass

func detonator_visibility(setVisible: bool):
	detonatorUI.visible = setVisible
	pass
