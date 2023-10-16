extends TileMap

var neighborDirectionIds
var difficulty
var bombCoords
var exploredCoords
var cellLabels
var cellLabelsParent
var rng

func _ready():
	var gameData = get_node("/root/GameData");
	
	neighborDirectionIds = gameData.get("neighborDirectionIds")
	difficulty = gameData.get("difficulty")
	bombCoords = gameData.get("bombCoords")
	exploredCoords = gameData.get("exploredCoords")
	cellLabels = gameData.get("cellLabels")
	cellLabelsParent = gameData.get("cellLabelsParent")
	var mapSize = gameData.get("mapSize")
	
	rng = RandomNumberGenerator.new()
	
	var mapCoords = calculate_coords(mapSize)
	
	generate_cells(mapCoords, false)
	setup_cell_labels(mapCoords)
	pass

func calculate_coords(mapSize: Vector2i):
	var calculatedCoords = []
	
	for x in mapSize[0]:
		for y in mapSize[1]:
			calculatedCoords.append(Vector2i(x,y))
			
	return calculatedCoords

func generate_cells(coordsList: Array, allowOverwrite: bool = true):
	for coords in coordsList:
		if (!allowOverwrite and get_cell_tile_data(0, coords) != null):
			continue
		
		var tile = 0
		
		var chance = rng.randf_range(0.0,100.0)
		if (chance <= difficulty):
			bombCoords.append(coords)
			tile = 1
			
		set_cell(0, coords, 0, Vector2i(tile,0), 0)
	pass

func setup_cell_labels(coordsList: Array):
	for coords in coordsList:
		var newCellLabel = Label.new()
		cellLabelsParent.add_child(newCellLabel)
		var globalLocation = map_to_local(coords)
		newCellLabel.position = globalLocation
		cellLabels[coords] = newCellLabel
	pass

func update_cell_label(coords: Vector2i):
	cellLabels[coords].text = str(calculate_indicator(coords, false))
	pass

func get_neighbor(coords: Vector2i, direction: int = 0, global: bool = true):
	var nextCell = get_neighbor_cell(coords, neighborDirectionIds[direction])
	if (!global):
		return nextCell
	var nextGlobalCell = map_to_local(nextCell)
	return nextGlobalCell

func calculate_indicator(coords: Vector2i, checkExplored: bool = true):
	if (!exploredCoords.has(coords) and checkExplored):
		return "?"
	
	var surroundingBombs = 0
	for neighbor in get_surrounding_cells(coords):
		if (bombCoords.has(neighbor)):
			surroundingBombs += 1
	return surroundingBombs

func on_enter_cell(coords: Vector2i):
	if (bombCoords.has(coords)):
		print("GAME OVER")
	else:
		print("Safe cell")
		exploredCoords.append(coords)
		update_cell_label(coords)
	pass
