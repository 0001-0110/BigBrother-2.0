import pygame

from b_BLL.models.Position import Position
from b_BLL.models.MenuElement import MenuElement
from b_BLL.models.Text import Text
from b_BLL.models.Hitbox import Hitbox

class Button(MenuElement):

    def __init__(self,position,size,spriteHoverOff,spriteHoverOn=None,string="",message="",fontName="arial",color=(255,255,255)):

        MenuElement.__init__(self)
        pygame.font.init()

        self.position = position
        self.size = size        
        self.message = message
        self.mouseHover = False

        self.text = Text(string)

        self.spriteHoverOff = spriteHoverOff
        #if none spriteHoverOn is given, use spriteHoverOff
        if spriteHoverOn == None:
            self.spriteHoverOn = self.spriteHoverOff
        else:
            self.spriteHoverOn = spriteHoverOn
        self.sprite = self.spriteHoverOff
        self.rect = self.sprite.get_rect()

    def display(self,screen):
        #display the button
        if self.mouseHover == True:
            self.sprite = self.spriteHoverOn
        else:
            self.sprite = self.spriteHoverOff
        #keep the width/height ratio
        ratio = self.sprite.get_width()/self.sprite.get_height()
        heightPixel = screen.get_height()*self.size/100
        widthPixel = ratio * heightPixel
        #resize the button
        self.sprite = pygame.transform.scale(self.sprite,(int(widthPixel),int(heightPixel)))
        #updating rect of button
        self.rect = self.sprite.get_rect()
        self.rect.x, self.rect.y = self.coordPixel(self.sprite,self.position,screen)
        #display
        screen.blit(self.sprite,(self.rect.x,self.rect.y))
        self.text.display(screen,self.position,self.size*0.7)

    def press(self):
        pygame.event.post(self.message)