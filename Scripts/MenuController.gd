extends Node2D

@onready var sceneManager: SceneManager = get_node("/root/SceneManager")
@onready var gameData: GameData = get_node("/root/GameData")
@onready var overlayHandler = get_node("/root/Overlay")

@export var startButton: Button

func _ready():
	startButton.connect("button_up", Callable(self, "start_game"))
	pass

func start_game():
	gameData.saveData = GameData.SaveData.new()
	sceneManager.load_scene("Level")
	overlayHandler.overlay_visibility(true)
	pass
