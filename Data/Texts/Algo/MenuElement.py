import pygame

class MenuElement:

    def __init__(self):
        pass

    #take x and y in percent
    #position : Position(x,y)
    #screen : pygame.display
    #return x and y in pixels
    def coordPixel(self,surface,position,screen):
        return int(screen.get_width() * position.x/100 - surface.get_width() // 2),int(screen.get_height() * position.y/100 - surface.get_height() // 2)