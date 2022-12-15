from b_BLL.models.Position import Position
from b_BLL.services.MovementService import MovementService

class Dirfinder:

    def __init__(self):
        pass

    def dirfinder(self,initialPosition,targetPosition,grid,obstacle=["+"]):
        
        if initialPosition.x == targetPosition.x and initialPosition.y == targetPosition.y:
            return None
        
        xMin = 0
        xMax = len(grid[0]) - 1
        yMin = 0
        yMax = len(grid) - 1

        #all moves
        moveUp = Position(0,-1)
        moveDown = Position(0,1)
        moveRight = Position(1,0)
        moveLeft = Position(-1,0)
        moves = [moveUp,moveDown,moveRight,moveLeft]

        #init list of already checked tiles
        tilesVisited = []
        for i in range(len(grid)):
            line = []
            for j in range(len(grid[0])):
                line.append(False)
            tilesVisited.append(line)
        tilesVisited[initialPosition.y][initialPosition.x] = True

        path = [initialPosition]
        paths = [path]
        
        while len(paths) > 0:
            pathsNew = []
            for path in paths:
                #print(path)     #DEBUG
                #print(len(path))
                for move in moves:
                    newPosition = Position(path[-1].x + move.x, path[-1].y + move.y)
                    #print("      ",newPosition)     #DEBUG
                    #check if new position is available
                    #this tile is in the grid
                    inTheGrid = newPosition.x >= xMin and newPosition.x <= xMax and newPosition.y >= yMin and newPosition.y <= yMax
                    if inTheGrid:
                        #this tile is walkable and has not already been checked
                        tileAvailable = grid[newPosition.y][newPosition.x] not in obstacle and tilesVisited[newPosition.y][newPosition.x] != True
                        if tileAvailable:
                            tilesVisited[newPosition.y][newPosition.x] = True
                            pathNew = path.copy()
                            pathNew.append(newPosition)
                            #print("       path created")    #DEBUG
                            #print("      ",newPath)  #DEBUG
                            #check if target position is reached
                            if newPosition.x == targetPosition.x and newPosition.y == targetPosition.y:
                                return pathNew
                            else:
                                pathsNew.append(pathNew)
            paths = pathsNew.copy()
            if len(paths) == 1:
                return paths[0]
            #print(paths)    #DEBUG
            #print(len(paths))       #DEBUG