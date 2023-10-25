extends TileMap

@export_category("Generator Dependencies")
@export var itemsMap: TileMap

@export_category("Generator Settings")
@export var borderSize: int = 4
@export var spawnPoint = Vector2i(5,5)

@onready var gameData: GameData = get_node("/root/GameData");

@onready var cellLabelPrefab = preload("res://Prefabs/UI/CellLabel.tscn")
@onready var cellLabelsParent = get_node("CellLabels")
var cellLabels = {}

@onready var collectedItems = gameData.get("collectedItems")

@onready var rng = RandomNumberGenerator.new()

var cellsToWin: int

func _ready():
	var mapCoords = calculate_coords(gameData.saveData.mapSize, borderSize)
	cellsToWin = mapCoords.size()
	setup_cell_labels(mapCoords)
	
	if (gameData.loadSave):
		print("Loading savedata")
		cellsToWin -= gameData.saveData.bombCoords.size()
		for cell in gameData.saveData.exploredCoords:
			if (check_bounds(cell, gameData.saveData.mapSize)):
				update_cell_label(cell)
	
	generate_cells(mapCoords, false, gameData.loadSave)
	pass

func calculate_coords(size, border: int = 0):
	var calculatedCoords = []
	
	for x in size[0] + border * 2 + 1:
		for y in size[1] + border * 2 + 1:
			calculatedCoords.append(Vector2i(x - border,y - border))
			
	return calculatedCoords

func generate_cells(coordsList: Array, allowOverwrite: bool = true, fromSaveData: bool = false):
	for coords in coordsList:
		if (!allowOverwrite and get_cell_tile_data(0, coords) != null):
			continue
		
		var tile = "Unexplored"
		
		var chance = rng.randf_range(0.0,100.0)
		if (!check_bounds(coords)):
			if (!fromSaveData):
				gameData.saveData.exploredCoords.append(coords)
			tile = "NonEnterable"
		elif (chance <= gameData.saveData.difficulty * 0.1 && !fromSaveData):
			gameData.saveData.itemCoords[coords] = "Coin"
		elif (chance <= gameData.saveData.difficulty && !fromSaveData):
			gameData.saveData.bombCoords.append(coords)
			cellsToWin -= 1
			#tile = "NonEnterable"
		
		if (fromSaveData):
			for flag in gameData.saveData.flagCoords:
				itemsMap.set_cell(0, flag, 1, gameData.itemTiles["Flag"])
		
		set_cell(0, coords, 0, gameData.roomTiles[tile])
	pass

func check_bounds(coords: Vector2i, size: Vector2i = gameData.saveData.mapSize):
	var outBounds = coords[0] > size[0] or coords[0] < 0 or coords[1] > size[1] or coords[1] < 0
	return !outBounds

func set_flag(coords: Vector2i):
	if (gameData.saveData.exploredCoords.has(coords)):
		return
	
	if (gameData.saveData.flagCoords.has(coords)):
		itemsMap.set_cell(0, coords)
		gameData.saveData.flagCoords.erase(coords)
	else:
		itemsMap.set_cell(0, coords, 1, gameData.itemTiles["Flag"])
		gameData.saveData.flagCoords.append(coords)
	pass

func setup_cell_labels(coordsList: Array):
	for coords in coordsList:
		cellLabelsParent.add_child(cellLabelPrefab.instantiate())
		var newCellLabel = cellLabelsParent.get_child(cellLabelsParent.get_child_count() - 1)
		var globalLocation = map_to_local(coords)
		newCellLabel.position = globalLocation - Vector2(128,128)
		cellLabels[coords] = newCellLabel
	pass

func update_cell_label(coords: Vector2i, updateRecursive := false):
	if (gameData.saveData.itemCoords.has(coords)):
		itemsMap.set_cell(0, coords, 1, gameData.itemTiles[gameData.saveData.itemCoords[coords]])
	
	if (gameData.saveData.flagCoords.has(coords)):
		itemsMap.set_cell(0, coords)
		
	set_cell(0, coords, 0, gameData.roomTiles["Explored"])
	
	var newIndicator = calculate_indicator(coords, false)
	if (newIndicator != "" or !updateRecursive):
		cellLabels[coords].text = newIndicator
		return
	
	var neighbors = get_surrounding_cells(coords)
	
	for neighbor in neighbors:
		if (!gameData.saveData.exploredCoords.has(neighbor)):
			gameData.saveData.exploredCoords.append(neighbor)
			update_cell_label(neighbor, true)
	pass

func get_neighbor(coords: Vector2i, direction: int = 0):
	var nextCell = get_neighbor_cell(coords, gameData.neighborDirectionIds[direction])
	return nextCell

func calculate_indicator(coords: Vector2i, checkExplored: bool = true):
	if (!gameData.saveData.exploredCoords.has(coords) and checkExplored):
		return ""
	
	var surroundingBombs = 0
	for neighbor in get_surrounding_cells(coords):
		if (gameData.saveData.bombCoords.has(neighbor)):
			surroundingBombs += 1
	
	var indicatorText = ""
	if (surroundingBombs > 0):
		indicatorText = str(surroundingBombs)
	return indicatorText

func on_enter_cell(coords: Vector2i):
	print(str(gameData.saveData.exploredCoords.size()) + " >= " + str(cellsToWin))
	
	if (gameData.saveData.itemCoords.has(coords)):
		print("Found item: " + gameData.saveData.itemCoords[coords])
		gameData.saveData.collectedItems[gameData.saveData.itemCoords[coords]] += 1
		itemsMap.set_cell(0, coords, -1)
		gameData.saveData.itemCoords.erase(coords)
	if (gameData.saveData.bombCoords.has(coords)):
		return false
	elif (gameData.saveData.exploredCoords.size() >= cellsToWin):
		return true
	elif (!gameData.saveData.exploredCoords.has(coords)):
		gameData.saveData.exploredCoords.append(coords)
		update_cell_label(coords, true)
		if (gameData.saveData.exploredCoords.size() >= cellsToWin):
			return true
	return null
