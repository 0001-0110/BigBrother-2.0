import pygame
from random import randint

from b_BLL.models.Position import Position
from b_BLL.models.Direction import Direction
from b_BLL.models.Guard import Behavior
from b_BLL.models.Item import ItemType

from b_BLL.services.Pathfinder import Pathfinder

class MovementService:

    def __init__(self):
        #all moves
        self.moveUp = Position(0,-1)
        self.moveDown = Position(0,1)
        self.moveRight = Position(1,0)
        self.moveLeft = Position(-1,0)
        self.moves = [self.moveUp,self.moveDown,self.moveRight,self.moveLeft]

        self.pathfinder = Pathfinder()

    def playerPositionNew(self,player,movement,level):
        positionNew = Position(player.position.x + movement.x,player.position.y + movement.y)
        #check if not in the grid
        if not level.positionInTheMap(positionNew):
            return player.position
        #check if a wall
        if level.cell(positionNew) == level.symbolWall:
            return player.position
        #check if a guard
        if self.positionHasGuard(positionNew,level):
            #player caught
            print("player caught")
            pygame.quit()
        #check if an exit
        if level.cell(positionNew) == level.symbolExit:
            if player.hasKey:
                #player wins
                print("XXXXXXX player wins")
                pygame.quit()
            else:
                return player.position

        #pick up item
        itemIndex =  self.positionHasItem(positionNew,level)
        if itemIndex != -1:     #why ?
            player.inventory.append(level.items[itemIndex])
            if level.items[itemIndex].type == ItemType.KEY:
                player.hasKey = True
            del level.items[itemIndex]
        
        #duplicate code (remove ?)
        """
        #check if reach the exit
        if level.cell(positionNew) == level.symbolExit:
            #check if all items has been colected
            if len(self.items) == 0:
            pass
        """
        return positionNew

    def guardPositionNew(self,guard,player,level):

        #choose a random path
        if guard.behavior == Behavior.PATROLLER:
            #position dans la direction opposee
            positionOpposite = guard.position.new(Direction.opposite(guard.direction))
            directionAvailable = []
            #on boucle sur les 4 directions
            for direction in Direction.list():
                positionTest = guard.position.new(direction)
                if level.positionInTheMap(positionTest):
                    if level.cell(positionTest) == level.symbolPath:
                    #if level.map[positionTest.y][positionTest.x] == level.symbolPath:
                        if not self.positionHasGuard(positionTest,level):
                            if not positionTest.equals(positionOpposite):
                                #la direction est disponible
                                directionAvailable.append(direction)

            if len(directionAvailable) == 0:
                guard.direction = Direction.opposite(guard.direction)
                positionNew = guard.position.new(Direction.opposite(guard.direction))

            else:
                guard.direction = directionAvailable[randint(0,len(directionAvailable)-1)]
                positionNew = guard.position.new(guard.direction)

        #take the shortest path to the player
        if guard.behavior == Behavior.HUNTER:
            #find the shortest path to the player
            guard.path = self.pathfinder.dirfinder(guard.position,player.position,level)
            if guard.path == None:
                #if there is no path, guard don't move
                return guard.position
            positionNew = guard.path[1]

        #
        elif guard.behavior == Behavior.DEFENDER:
            playerPath = self.pathfinder.pathfinder(player.position,guard.target,level)
            if self.containsPosition(playerPath,guard.position) != -1:
                #guard is already on the shortest path between player and exit
                tileTarget = player.position
            else:
                #guard is not on the shortest path between player and exit
                tileTarget = playerPath[int(len(playerPath)/2)]
            #find the shortest path to tileTarget
            guard.path = self.pathfinder.dirfinder(guard.position,tileTarget,level)
            if guard.path == None:
                #if there is no path, guard don't move
                return guard.position
            positionNew = guard.path[1]

        #if player caught
        if player.position.equals(positionNew):
            print("player caught")
            pygame.quit()
            pass

        if self.positionHasGuard(positionNew,level) or level.cell(positionNew) == level.symbolWall:
        #if self.positionHasGuard(positionNew,level) or level.map[positionNew.y][positionNew.x] == level.symbolWall:
            return guard.position

        return positionNew
        
    """
    def newPosition(self,position,direction):
        return Position(position.x + direction.x,position.y + direction.y)
    """

    def containsPosition(self,positionList,position):
        #if list contains Position, return Position index
        #else, return -1
        for i in range(len(positionList)):
            if position.x == positionList[i].x and position.y == positionList[i].y:
                return i
        return -1

    def positionHasGuard(self,position,level):
        #check if guard is blocking the way
        #otherGuard include himself
        for otherGuard in level.guards:
            if position.equals(otherGuard.position):
                #wait until the way is clear
                return True
        return False

    def positionHasItem(self,position,level):
        for index in range(len(level.items)):
            if position.equals(level.items[index].position):
                #return index of item
                return index
        #si aucun tresor sur cette case
        return -1