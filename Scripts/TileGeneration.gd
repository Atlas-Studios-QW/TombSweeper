extends TileMap

func _ready():
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
			
		set_cell(0, coords, 0, Vector2i(1,0), 0)
	pass

func GetGlobalNeighbor(location: Vector2):
	return
