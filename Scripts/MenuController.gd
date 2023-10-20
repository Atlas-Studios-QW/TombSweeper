extends Node2D

@onready var sceneManager = get_node("/root/SceneManager")

@export var startButton: Button

func _ready():
	startButton.connect("button_up", Callable(sceneManager, "load_scene").bind("Level"))
	pass
