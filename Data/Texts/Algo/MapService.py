from random import randint

from c_DAL.MapDAO import MapDAO
from b_BLL.models.Position import Position
from b_BLL.models.Direction import Direction
from b_BLL.models.Level import Level
from b_BLL.models.Player import Player
from b_BLL.models.Guard import Guard,Behavior
from b_BLL.models.Item import Item
from b_BLL.models.Exit import Exit

from b_BLL.services.CalculationService import distanceSquare

class MapService:

    def __init__(self,mapFile):
        self.level = Level()
        self.mapHandler(mapFile)

    def mapHandler(self,mapFile):
        self.mapDAO = MapDAO()
        self.level.map = self.mapDAO.mapLoad(mapFile)

        self.level.tilesHorizontal = len(self.level.map[0])
        self.level.tilesVertical = len(self.level.map)

        #scan entire map to find symbols
        for Y in range(len(self.level.map)):
            for X in range(len(self.level.map[Y])):
                #find start Positions of player
                if self.level.map[Y][X] == self.level.symbolPlayer:
                    self.player = Player(Position(X,Y))
                    self.level.map[Y][X] = self.level.symbolPath
                #find start Positions of guards (patrollers)
                elif self.level.map[Y][X] == self.level.symbolGuardPatroller:
                    self.level.guards.append(Guard(Position(X,Y),Behavior.PATROLLER))
                    self.level.map[Y][X] = self.level.symbolPath
                #find start Positions of guards (hunters)
                elif self.level.map[Y][X] == self.level.symbolGuardHunter:
                    self.level.guards.append(Guard(Position(X,Y),Behavior.HUNTER))
                    self.level.map[Y][X] = self.level.symbolPath
                #find start Positions of guards (defenders)
                elif self.level.map[Y][X] == self.level.symbolGuardDefender:
                    self.level.guards.append(Guard(Position(X,Y),Behavior.DEFENDER))
                    self.level.map[Y][X] = self.level.symbolPath
                #find item positions
                elif self.level.map[Y][X] == self.level.symbolItem:
                    self.level.items.append(Item(Position(X,Y)))
                    self.level.map[Y][X] = self.level.symbolPath
                #find exit Positions
                elif self.level.map[Y][X] == self.level.symbolExit:
                    self.level.exits.append(Exit(Position(X,Y)))

        #handle guards behavior
        for guard in self.level.guards:
            if guard.behavior == Behavior.DEFENDER:
                #selecting the closest exit
                guard.target = self.level.exits[0].position
                distanceMin = distanceSquare(guard.position,guard.target)
                for myExit in self.level.exits:
                    distance =  distanceSquare(guard.position,myExit.position) 
                    if distance < distanceMin:
                        guard.target = myExit.position
                        distanceMin = distance
            elif guard.behavior == Behavior.PATROLLER:
                #init a random direction
                guard.direction = Direction.random()