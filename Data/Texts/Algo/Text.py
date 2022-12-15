import pygame
from b_BLL.models.MenuElement import MenuElement

class Text(MenuElement):

    #screen : tuple (screenWidth,screenHeight)
    def __init__(self,string,color=(255,255,255),fontName="arial"):
        MenuElement.__init__(self)
        self.string = string
        self.color = color
        self.fontName = fontName
        
    #display text
    #color (r,v,b) 
    #size, in percent
    def display(self,screen,position,size):
        font = pygame.font.SysFont(self.fontName, int(screen.get_height() * size/100))
        text = font.render(self.string, True, self.color)
        screen.blit(text,self.coordPixel(text,position,screen))