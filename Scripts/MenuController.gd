extends Node2D

@onready var sceneManager: SceneManager = get_node("/root/SceneManager")
@onready var gameData: GameData = get_node("/root/GameData")
@onready var overlayHandler = get_node("/root/Overlay")

@export_category("Options")
@export var startButton: Button
@export var loadButton: Button
@export var quitButton: Button
@export var mapSizeX: SpinBox
@export var mapSizeY: SpinBox
@export var difficulty: OptionButton

func _ready():
	startButton.connect("button_up", Callable(self, "start_game"))
	quitButton.connect("button_up", Callable(self, "quit"))
	
	if ResourceLoader.exists("user://savegame.res"):
		loadButton.visible = true
		loadButton.connect("button_up", Callable(self, "load_game"))
	else:
		loadButton.visible = false
	pass

func start_game():
	@warning_ignore("narrowing_conversion")
	gameData.loadSave = false
	gameData.saveData = SaveData.new(Vector2i(mapSizeX.value, mapSizeY.value), SaveData.Difficulty[difficulty.text])
	sceneManager.load_scene("Level")
	overlayHandler.overlay_visibility(true)
	pass

func load_game():
	gameData.loadSave = true
	gameData.load_progress()
	sceneManager.load_scene("Level")
	overlayHandler.overlay_visibility(true)
	pass

func quit():
	get_tree().quit()
	pass
