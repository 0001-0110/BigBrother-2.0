class Position:

    def __init__(self,x=None,y=None,z=None):
        self.x = x
        self.y = y
        self.z = z

    def new(self,direction):
        return Position(self.x + direction.x,self.y + direction.y)

    def equals(self,position):
        return self.x == position.x and self.y == position.y

    #unfinished
    def isIn(self,rect):
        return self.x 