extends TileMap

@export var itemsMap: TileMap

var neighborDirectionIds
var difficulty
var mapSize

var roomTiles
var itemTiles

var itemCoords
var flagCoords
var bombCoords
var exploredCoords

var cellLabels
var cellLabelsParent
var cellLabelPrefab

var collectedItems

var rng

func _ready():
	var gameData = get_node("/root/GameData");
	
	neighborDirectionIds = gameData.get("neighborDirectionIds")
	roomTiles = gameData.get("roomTiles")
	itemTiles = gameData.get("itemTiles")
	
	difficulty = gameData.get("difficulty")
	mapSize = gameData.get("mapSize")
	
	itemCoords = gameData.get("itemCoords")
	flagCoords = gameData.get("flagCoords")
	bombCoords = gameData.get("bombCoords")
	exploredCoords = gameData.get("exploredCoords")
	
	collectedItems = gameData.get("collectedItems")
	
	cellLabelPrefab = preload("res://Prefabs/UI/CellLabel.tscn")
	cellLabels = gameData.get("cellLabels")
	cellLabelsParent = gameData.get("cellLabelsParent")
	
	rng = RandomNumberGenerator.new()
	
	var mapCoords = calculate_coords(4)
	
	generate_cells(mapCoords, false)
	setup_cell_labels(mapCoords)
	pass

func calculate_coords(borderSize: int = 0):
	var calculatedCoords = []
	
	for x in mapSize[0] + borderSize * 2:
		for y in mapSize[1] + borderSize * 2:
			calculatedCoords.append(Vector2i(x - borderSize,y - borderSize))
			
	return calculatedCoords

func generate_cells(coordsList: Array, allowOverwrite: bool = true):
	for coords in coordsList:
		if (!allowOverwrite and get_cell_tile_data(0, coords) != null):
			continue
		
		var tile = "Unexplored"
		
		var chance = rng.randf_range(0.0,100.0)
		if (!check_bounds(coords)):
			bombCoords.append(coords)
			tile = "NonEnterable"
		elif (chance <= difficulty * 0.1):
			itemCoords[coords] = "Coin"
		elif (chance <= difficulty):
			bombCoords.append(coords)
			#tile = "NonEnterable"
			
		set_cell(0, coords, 0, roomTiles[tile])
	
	print (itemCoords)
	pass

func check_bounds(coords: Vector2i, size: Vector2i = mapSize):
	var outBounds = coords[0] > size[0] or coords[0] < 0 or coords[1] > size[1] or coords[1] < 0
	return !outBounds

func set_flag(coords: Vector2i):
	if (exploredCoords.has(coords)):
		return
	
	if (flagCoords.has(coords)):
		itemsMap.set_cell(0, coords)
		flagCoords.erase(coords)
	else:
		itemsMap.set_cell(0, coords, 1, itemTiles["Flag"])
		flagCoords.append(coords)
	pass

func setup_cell_labels(coordsList: Array):
	for coords in coordsList:
		cellLabelsParent.add_child(cellLabelPrefab.instantiate())
		var newCellLabel = cellLabelsParent.get_child(cellLabelsParent.get_child_count() - 1)
		var globalLocation = map_to_local(coords)
		newCellLabel.position = globalLocation - Vector2(16,16)
		cellLabels[coords] = newCellLabel
	pass

func update_cell_label(coords: Vector2i, updateRecursive: bool = false):
	if (itemCoords.has(coords)):
		itemsMap.set_cell(0, coords, 1, itemTiles[itemCoords[coords]])
	
	if (flagCoords.has(coords)):
		itemsMap.set_cell(0, coords)
		
	set_cell(0, coords, 0, roomTiles["Explored"])
	
	var newIndicator = calculate_indicator(coords, false)
	if (newIndicator != "" or !updateRecursive):
		cellLabels[coords].text = newIndicator
		return
	
	var neighbors = get_surrounding_cells(coords)
	
	for neighbor in neighbors:
		if (!exploredCoords.has(neighbor)):
			exploredCoords.append(neighbor)
			update_cell_label(neighbor, true)
	
	pass

func get_neighbor(coords: Vector2i, direction: int = 0):
	var nextCell = get_neighbor_cell(coords, neighborDirectionIds[direction])
	return nextCell

func calculate_indicator(coords: Vector2i, checkExplored: bool = true):
	if (!exploredCoords.has(coords) and checkExplored):
		return ""
	
	var surroundingBombs = 0
	for neighbor in get_surrounding_cells(coords):
		if (bombCoords.has(neighbor)):
			surroundingBombs += 1
	
	var indicatorText = ""
	if (surroundingBombs > 0):
		indicatorText = str(surroundingBombs)
	return indicatorText

func on_enter_cell(coords: Vector2i):
	if (itemCoords.has(coords)):
		print("Found item: " + itemCoords[coords])
		collectedItems[itemCoords[coords]] += 1
		itemsMap.set_cell(0, coords, -1)
		itemCoords.erase(coords)
	
	if (bombCoords.has(coords)):
		print("GAME OVER")
		print("Total coins: " + str(collectedItems["Coin"]))
	else:
		print("Safe cell")
		exploredCoords.append(coords)
		update_cell_label(coords, true)
	pass
