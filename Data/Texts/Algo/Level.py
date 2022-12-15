class Level:

    def __init__(self):
        #const
        self.symbolPath = " "
        self.symbolWall = "+"
        self.symbolPlayer = "X"
        self.symbolGuardPatroller = "P"
        self.symbolGuardHunter = "H"
        self.symbolGuardDefender = "D"
        self.symbolItem = "I"
        self.symbolExit = "E"
        #
        self.map = None
        #number of tiles
        self.tilesHorizontal = None
        self.tilesVertical = None
        self.items = []
        self.exits = []

        self.guards = []

    def cell(self,position):
        return self.map[position.y][position.x]

    def positionInTheMap(self,position):

        xMin = 0
        xMax = len(self.map[0]) - 1
        yMin = 0
        yMax = len(self.map) - 1

        return position.x >= xMin and position.x <= xMax and position.y >= yMin and position.y <= yMax

    def directionInTheMap(self,position,direction):

        xMin = 0
        xMax = len(self.map[0]) - 1
        yMin = 0
        yMax = len(self.map) - 1

        return (position.x + direction.x) >= xMin and (position.x + direction.x) <= xMax and (position.y + direction.y) >= yMin and (position.y + direction.y) <= yMax