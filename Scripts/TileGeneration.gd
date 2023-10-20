extends TileMap

@export_category("Generator Dependencies")
@export var itemsMap: TileMap

@export_category("Generator Settings")
@export var borderSize: int = 4
@export var spawnPoint = Vector2i(5,5)

@onready var gameData = get_node("/root/GameData");
@onready var saveData: GameData.SaveData = gameData.saveData

@onready var neighborDirectionIds = gameData.get("neighborDirectionIds")
@onready var roomTiles = gameData.roomTiles
@onready var itemTiles = gameData.itemTiles

@onready var cellLabelPrefab = preload("res://Prefabs/UI/CellLabel.tscn")
@onready var cellLabelsParent = get_node("CellLabels")

@onready var collectedItems = gameData.get("collectedItems")

@onready var rng = RandomNumberGenerator.new()

var cellsToWin: int

func _ready():
	var mapCoords = calculate_coords(saveData.mapSize, borderSize)
	cellsToWin = mapCoords.size()
	generate_cells(mapCoords, false)
	setup_cell_labels(mapCoords)
	pass

func calculate_coords(size, border: int = 0):
	var calculatedCoords = []
	
	for x in size[0] + border * 2 + 1:
		for y in size[1] + border * 2 + 1:
			calculatedCoords.append(Vector2i(x - border,y - border))
			
	return calculatedCoords

func generate_cells(coordsList: Array, allowOverwrite: bool = true):
	for coords in coordsList:
		if (!allowOverwrite and get_cell_tile_data(0, coords) != null):
			continue
		
		var tile = "Unexplored"
		
		var chance = rng.randf_range(0.0,100.0)
		if (!check_bounds(coords)):
			saveData.exploredCoords.append(coords)
			tile = "NonEnterable"
		elif (chance <= saveData.difficulty * 0.1):
			saveData.itemCoords[coords] = "Coin"
		elif (chance <= saveData.difficulty):
			saveData.bombCoords.append(coords)
			cellsToWin -= 1
			#tile = "NonEnterable"
			
		set_cell(0, coords, 0, roomTiles[tile])
	
	print (saveData.itemCoords)
	pass

func check_bounds(coords: Vector2i, size: Vector2i = saveData.mapSize):
	var outBounds = coords[0] > size[0] or coords[0] < 0 or coords[1] > size[1] or coords[1] < 0
	return !outBounds

func set_flag(coords: Vector2i):
	if (saveData.exploredCoords.has(coords)):
		return
	
	if (saveData.flagCoords.has(coords)):
		itemsMap.set_cell(0, coords)
		saveData.flagCoords.erase(coords)
	else:
		itemsMap.set_cell(0, coords, 1, itemTiles["Flag"])
		saveData.flagCoords.append(coords)
	pass

func setup_cell_labels(coordsList: Array):
	for coords in coordsList:
		cellLabelsParent.add_child(cellLabelPrefab.instantiate())
		var newCellLabel = cellLabelsParent.get_child(cellLabelsParent.get_child_count() - 1)
		var globalLocation = map_to_local(coords)
		newCellLabel.position = globalLocation - Vector2(128,128)
		saveData.cellLabels[coords] = newCellLabel
	pass

func update_cell_label(coords: Vector2i, updateRecursive: bool = false):
	if (saveData.itemCoords.has(coords)):
		itemsMap.set_cell(0, coords, 1, gameData.itemTiles[saveData.itemCoords[coords]])
	
	if (saveData.flagCoords.has(coords)):
		itemsMap.set_cell(0, coords)
		
	set_cell(0, coords, 0, roomTiles["Explored"])
	
	var newIndicator = calculate_indicator(coords, false)
	if (newIndicator != "" or !updateRecursive):
		saveData.cellLabels[coords].text = newIndicator
		return
	
	var neighbors = get_surrounding_cells(coords)
	
	for neighbor in neighbors:
		if (!saveData.exploredCoords.has(neighbor)):
			saveData.exploredCoords.append(neighbor)
			update_cell_label(neighbor, true)
	pass

func get_neighbor(coords: Vector2i, direction: int = 0):
	var nextCell = get_neighbor_cell(coords, neighborDirectionIds[direction])
	return nextCell

func calculate_indicator(coords: Vector2i, checkExplored: bool = true):
	if (!saveData.exploredCoords.has(coords) and checkExplored):
		return ""
	
	var surroundingBombs = 0
	for neighbor in get_surrounding_cells(coords):
		if (saveData.bombCoords.has(neighbor)):
			surroundingBombs += 1
	
	var indicatorText = ""
	if (surroundingBombs > 0):
		indicatorText = str(surroundingBombs)
	return indicatorText

func on_enter_cell(coords: Vector2i):
	if (saveData.itemCoords.has(coords)):
		print("Found item: " + saveData.itemCoords[coords])
		saveData.collectedItems[saveData.itemCoords[coords]] += 1
		itemsMap.set_cell(0, coords, -1)
		saveData.itemCoords.erase(coords)
	if (saveData.bombCoords.has(coords)):
		return false
	elif (saveData.exploredCoords.size() >= cellsToWin):
		return true
	elif (!saveData.exploredCoords.has(coords)):
		saveData.exploredCoords.append(coords)
		update_cell_label(coords, true)
		if (saveData.exploredCoords.size() >= cellsToWin):
			return true
	return null
