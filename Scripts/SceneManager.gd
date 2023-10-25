extends Node

var scenes = {
	"MainMenu" = "res://Scenes/MainMenu.tscn",
	"Level" = "res://Scenes/Level.tscn"
}

var currentScene = null

func _ready():
	var root = get_tree().root
	currentScene = root.get_child(root.get_child_count() - 1)

func load_scene(sceneName: String):
	if (!scenes[sceneName]):
		printerr("Scene not found!")
		return
	
	call_deferred("deferred_load_scene", sceneName)
	pass

func deferred_load_scene(sceneName: String):
	get_tree().root.remove_child(currentScene)
	
	currentScene.free()
	
	var newScene = ResourceLoader.load(scenes[sceneName])
	
	currentScene = newScene.instantiate()
	
	get_tree().root.add_child(currentScene)
	get_tree().current_scene = currentScene
	pass
