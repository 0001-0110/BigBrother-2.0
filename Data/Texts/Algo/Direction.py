from enum import Enum
from random import randint

from b_BLL.models.Position import Position

class Direction(Enum):
    UP = Position(0,-1)
    DOWN = Position(0,1)
    RIGHT = Position(1,0)
    LEFT = Position(-1,0)

    @staticmethod
    def list():
        return list(map(lambda c: c.value, Direction))

    @staticmethod
    def random():
        myList = Direction.list()
        return myList[randint(0,len(myList)-1)]

    @staticmethod
    def opposite(direction):
        return Position(direction.x*-1,direction.y*-1)