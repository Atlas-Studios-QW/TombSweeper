extends CanvasLayer

var gameData

var animator: AnimationPlayer

@export var coinsCounter: Label

func _ready():
	gameData = get_node("/root/GameData")
	animator = get_node("OverlayAnimator")
	pass

func _update_from_gamedata():
	_update_coin_counter(gameData.get("collectedItems")["Coin"])
	pass

func _update_coin_counter(newAmount: int):
	var newText = "Coins: " + str(newAmount)
	if (newText != coinsCounter.text):
		animator.play("CoinCollected")
	coinsCounter.text = newText
	pass
