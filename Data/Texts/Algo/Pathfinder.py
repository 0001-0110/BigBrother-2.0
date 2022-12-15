#Created by TROTEL "22" Rémi
#21/03/2020
#patfinder function
#need initial Position, target Position, grid, list of all walkable type of tile (optional)
#return the shortest path (list of Position)
#first Position in return path is initial Position
#last Position in return path is target Position
#all positions are Position objects with attributs x and y
#if there is no possible path, return None

from b_BLL.models.Position import Position

class Pathfinder:

    def __init__(self):
        pass

    def pathfinder(self,initialPosition,targetPosition,level,obstacle=["+"]):

        if initialPosition.x == targetPosition.x and initialPosition.y == targetPosition.y:
            return None

        #all moves
        moveUp = Position(0,-1)
        moveDown = Position(0,1)
        moveRight = Position(1,0)
        moveLeft = Position(-1,0)
        moves = [moveUp,moveDown,moveRight,moveLeft]

        #init list of already checked tiles
        tilesVisited = []
        for i in range(len(level.map)):
            line = []
            for j in range(len(level.map[0])):
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
                    if level.positionInTheMap(newPosition):
                        #this tile is walkable and has not already been checked
                        tileAvailable = level.map[newPosition.y][newPosition.x] not in obstacle and tilesVisited[newPosition.y][newPosition.x] != True
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
            #print(paths)    #DEBUG
            #print(len(paths))       #DEBUG

    def dirfinder(self,initialPosition,targetPosition,level,obstacle=["+"]):
        
        if initialPosition.x == targetPosition.x and initialPosition.y == targetPosition.y:
            return None

        #all moves
        moveUp = Position(0,-1)
        moveDown = Position(0,1)
        moveRight = Position(1,0)
        moveLeft = Position(-1,0)
        moves = [moveUp,moveDown,moveRight,moveLeft]

        #init list of already checked tiles
        tilesVisited = []
        for i in range(len(level.map)):
            line = []
            for j in range(len(level.map[0])):
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
                    if level.positionInTheMap(newPosition):
                        #this tile is walkable and has not already been checked
                        tileAvailable = level.map[newPosition.y][newPosition.x] not in obstacle and tilesVisited[newPosition.y][newPosition.x] != True
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
