import pygame
from enum import Enum

from b_BLL.models.Position import Position

class Behavior(Enum):
    PATROLLER = 10
    HUNTER = 20
    DEFENDER = 30

class Guard:
    count = 0

    def __init__(self,position=Position(),behavior=Behavior.HUNTER):
        self.count += 1
        self.id = self.count
        self.position = position
        #sprite
        self.sprite = pygame.image.load("a_HMI/assets/sprites/titan_2x.png")
        #chasing behavior
        self.behavior = behavior
        #select a target Position
        self.target = None
        #path followed by this guard
        self.path = [self.position]
        #direction of the guard
        self.direction = None
        #init event
        self.delay = 250
        self.event = pygame.USEREVENT + 1 + self.id

    #start the timer of events of this guard
    def timerStart(self):
        pygame.time.set_timer(self.event,self.delay)