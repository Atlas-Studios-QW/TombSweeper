extends CharacterBody2D

var playerSpeed

func _ready():
	playerSpeed = get_node("/root/GameData").get("playerSpeed")
	pass

func _physics_process(delta):
	move_and_slide()
	pass
