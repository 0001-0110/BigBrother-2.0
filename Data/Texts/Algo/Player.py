import pygame

from b_BLL.models.Position import Position

class Player:

    def __init__(self,position=Position()):
        self.position = position
        self.inventory = []
        self.hasKey = False
        #sprite
        self.sprite = pygame.image.load("a_HMI/assets/sprites/Idle__000.png")
        self.delay = 150
        self.event = pygame.USEREVENT + 1
        pygame.time.set_timer(self.event,self.delay)