import pygame
from enum import Enum

from b_BLL.models.Position import Position

class ItemType(Enum):
    KEY = 10

class Item:

    def __init__(self,position=Position(),myType=ItemType.KEY):
        self.position = position
        self.type = myType
        #sprite
        self.sprite = pygame.image.load("a_HMI/assets/sprites/Icon-coin.png")