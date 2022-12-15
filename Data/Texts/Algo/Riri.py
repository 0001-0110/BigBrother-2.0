from random import random, choice

class Frequency_dict:

    def __init__(self,depth:int=10) -> None:
        self.dict = {}
        self.depth = depth

    def __str__(self) -> str:
        return str(self.dict)
    
    def __len__(self) -> int:
        return len(self.dict)

    def __contains__(self,value):
        return value in self.dict

    def append(self,previous_letters,following_letter,value:int=1) -> None:
        """add a value to the dict"""
        if not previous_letters in self.dict.keys():
            #these previous letters are not yet in this dict
            self.dict[previous_letters] = [{following_letter : value}, value]
        else:
            if not following_letter in self.dict[previous_letters][0]:
                #previous letters are already in the dict, but not the following letter
                self.dict[previous_letters][0][following_letter] = value
            else:
                #previous letters and following letters are both already in the dict, we just add the value
                self.dict[previous_letters][0][following_letter] += value
            #we add the value in the list, counting the total number of occurences
            self.dict[previous_letters][1] += value

    def __getattr__(self,attr):
        return self.dict[attr]

    def __add__(self,frequency_dict):
        pass
    
    def __iadd__(self,frequency_dict):
        for key in frequency_dict.dict:
            for character in frequency_dict.dict[key][0]:
                self.append(key,character,frequency_dict.dict[key][0][character])
        return self

    def __mul__(self,coef:int):
        """this method is not working"""
        new_dict = self.deepcopy()
        for key in new_dict.dict:
            for character in new_dict.dict[key][0]:
                new_dict[key][0][character] *= coef
            new_dict[key][1] *= coef
        return None

    def __imul__(self,coef:int):
        pass

class Riri:

    def analyze_text(self,text:str,depth:int=10) -> Frequency_dict:
        """analyse the given text and return a frequency dict, containing the number of occurences of each characters depending of the previous letters"""
        frequency_dict = Frequency_dict(depth)
        for i in range(len(text)):
            following_letter = text[i]
            for j in range(i-depth,i):
                previous_letters = text[j:i]
                if j < 0:
                    continue
                else:
                    frequency_dict.append(previous_letters,following_letter)
                    #print("{0} is followed by {1}".format(previous_letters,following_letter))      #DEBUG
        return frequency_dict

    def merge_frequency_dict(self,frequency_dict:Frequency_dict,*dicts:Frequency_dict) -> Frequency_dict:
        """merge multiples frequency dicts in a sigle one, regrouping all their values"""
        for other_dict in dicts:
            frequency_dict.depth = max(frequency_dict.depth,other_dict.depth)
            frequency_dict += other_dict
        return frequency_dict

    def analyze_texts(self,*texts:str,depth:int=10) -> Frequency_dict:
        frequency_dicts = []
        for text in texts:
            frequency_dicts.append(self.analyze_text(text,depth))
        return self.merge_frequency_dict(*frequency_dicts)

    def get_random_character(self,frequency_dict:Frequency_dict):
        """return a random character from the given frequency dict"""
        return choice(list(frequency_dict.dict[choice(list(frequency_dict.dict))][0]))

    def generate_next_letter(self,frequency_dict:Frequency_dict,previous_letters:str) -> str:
        """return a single letter, chosen according to the given frequency dict"""
        for i in range(len(previous_letters)):
            #perform the search by removing letters one by one until a match is found
            #TO DO : "string" is not a very clear variable name, might be better to change this
            string = previous_letters[i:]
            if string in frequency_dict.dict:
                #found a match
                random_value = random()
                value = 0
                for key in frequency_dict.dict[string][0].keys():
                    #chose the key to return, according to the frequencies in the dict
                    value += frequency_dict.dict[string][0][key]/frequency_dict.dict[string][1]
                    if random_value < value:
                        return key
        #the given dict contains no data matching the search
        #TO DO : the random character is a temporary fix, must be replaced by something else later
        return self.get_random_character(frequency_dict)

    def generate_text(self,frequency_dict:Frequency_dict,length:int=100) -> str:
        text = ""
        for i in range(length):
            text += self.generate_next_letter(frequency_dict,text[-frequency_dict.depth:])
            #print(text[-1],end="")     #DEBUG
        return text

if __name__ == "__main__":
    import os
    #this code will execute only if launched from this file
    riri = Riri()
    folder = "lyrics"
    artists_filter = ["charles_aznavour","francis_cabrel","daniel_balavoine"]
    lyrics_frequency = []
    for artist_folder in os.listdir(folder):
        if artist_folder in artists_filter:
            for song_file in os.listdir("/".join([folder,artist_folder])):
                file = open("/".join([folder,artist_folder,song_file]))
                lyrics_frequency.append(riri.get_frequency_dict(file.read(),5))

    frequency_dict = riri.merge_frequency_dict(*lyrics_frequency)

    text = riri.generate_text(frequency_dict,1000)
    print(text)