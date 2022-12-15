class Hitbox:

    def __init__(self,x,y,width=0,height=0):
        self.x = x
        self.y = y
        self.width = width
        self.height = height

    #take hitbox instance as parameter
    #return Bool
    #if hitbox are colliding return True 
    #else return False
    def colliding(self,hitbox):
        if (self.x < hitbox.x + hitbox.width and self.x > hitbox.x or self.x + self.width < hitbox.x + hitbox.width and self.x + self.width > hitbox.x and
        self.y < hitbox.y + hitbox.height and self.y > hitbox.y or self.y + self.height < hitbox.y + hitbox.height and self.y + self.height > hitbox.y):
            return True
        return False