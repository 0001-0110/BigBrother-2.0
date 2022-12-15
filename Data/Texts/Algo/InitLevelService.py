
from b_BLL.services.MapService import MapService

class InitLevelService:

    def __init__(self):

        #select map file
        self.mapFile = "d_DATA/maps/map_pacman.txt"
        mapService = MapService(self.mapFile)
        self.level = mapService.level
        #init player
        self.player = mapService.player
