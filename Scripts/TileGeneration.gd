extends TileMap

var neighborDirectionIds
var difficulty
var bombLocations

var rng

func _ready():
	neighborDirectionIds = get_node("/root/GameData").get("neighborDirectionIds")
	difficulty = get_node("/root/GameData").get("difficulty")
	bombLocations = get_node("/root/GameData").get("bombLocations")
	rng = RandomNumberGenerator.new()
	var mapSize = get_node("/root/GameData").get("mapSize")
	generate_cells(calculate_coords(mapSize), false)
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
			return
		
		var tile = 0
		
		var chance = rng.randf_range(0.0,100.0)
		if (chance <= difficulty):
			bombLocations.append(coords)
			tile = 1
			
		set_cell(0, coords, 0, Vector2i(tile,0), 0)
	pass

func GetNeighbor(location: Vector2, direction: int = 0, global: bool = true):
	var currentCell = local_to_map(location)
	var nextCell = get_neighbor_cell(currentCell, neighborDirectionIds[direction])
	if (!global):
		return nextCell
	var nextGlobalCell = map_to_local(nextCell)
	return nextGlobalCell

func GetGlobalPos(coords: Vector2i):
	var globalPos = map_to_local(coords)
	return globalPos

func CalculateIndicatorForCell(coords: Vector2i):
	var surroundingBombs = 0
	print(bombLocations)
	print("Cell: " + str(coords))
	for neighbor in get_surrounding_cells(coords):
		print("Neighbor: " + str(neighbor))
		if (bombLocations.has(neighbor)):
			surroundingBombs += 1
	print("Found: " + str(surroundingBombs))
	return surroundingBombs
