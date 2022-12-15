from typing import Union

# TO DO : add all the documentation
# TO DO : add all dependencies
# TO DO : upgrade the code to be able to handle n rotors (instead of 3)

# TO DO : needs tests

def obsolete(fun) -> callable:
    def wrapper(*args,**kwargs):
        print("WARNING : THIS FUNCTION IS OBSOLETE")
    return wrapper

def expected_types(*args_types:list,**kwargs_types:list) -> callable:
    def decorator(fun) -> callable:
        def wrapper(*args,**kwargs) -> callable:
            if len(args) != len(args_types):
                raise ValueError("")
            for i,arg in enumerate(args):
                if type(arg) != args_types[i]:
                    raise TypeError("{} has type {} but an expression was expected of type {}".format(arg,type(arg),args_types[i]))
            for key in kwargs:
                if key not in kwargs_types:
                    raise KeyError("")
                elif type(kwargs[key]) != type(kwargs[key]):
                    raise TypeError("{} has type {} but an expression was expected of type {}".format(key,type(kwargs[key]),type(kwargs[key])))
            return fun(*args,**kwargs)
        return wrapper
    return decorator

class Rotor:

    def init_connections(self,connections:Union[dict,str,list[tuple[Union[str,int]]]]) -> None:
        """TO DO"""
        # TO DO : need checks to be sure that all wires are there ?
        if type(connections) == dict:
            self.connections = connections
        elif type(connections) == str:
                connections = {}
                letters = [chr(i) for i in range(65, 65 + 26)]
                for i,letter in enumerate(letters):
                    self.connections[letters[i]] = connections[i]
        elif type(connections) == list:
            self.onnections = {}
            for letter1, letter2 in connections:
                if type(letter1) == str and type(letter2) == str and letter1 >= "A" and letter1 <= "Z" and letter2 >= "A" and letter2 <= "Z":
                    self.plugboard[self.letter_to_index(letter1)] = self.letter_to_index(letter2)
                    self.plugboard[self.letter_to_index(letter2)] = self.letter_to_index(letter1)
                elif type(letter1) == int and type(letter2) == int and letter1 >= 0 and letter1 <= 25 and letter2 >= 0 and letter2 <= 25:
                    self.plugboard[letter1] = letter2
                    self.plugboard[letter2] = letter1
                else:
                    raise ValueError("")
        else:
            raise ValueError("")

    def init_notches(self,notches:list) -> None:
        """TO DO"""
        self.notches = notches

    def set_position(self,position:int) -> None:
        """TO DO"""
        if position >= 0 and position <= 25:
            self.position = position
        else:
            raise ValueError("Are you trying to break something ?")

    def init(self,connections:dict,notches:list,position:int=0):
        """TO DO"""
        self.init_connections(connections)
        self.init_notches(notches)
        self.set_position(position)

    def __init__(self,connections:dict,notches:list,position:int=0) -> None:
        """TO DO"""
        self.init(connections,notches,position)

    def step(self,increment:int=1) -> None:
        """TO DO"""
        self.position += increment

    def inverse(self):
        """TO DO"""
        inversed_connections = {}
        for key, value in self.connections.items():
            inversed_connections[value] = key
        return Rotor(inversed_connections,self.notches,self.position)

    def encrypt(self,input:int) -> int:
        """TO DO"""
        return (self.connections[(input - self.position) % 26] + self.position) % 26

class Enigma:
    # TO DO : what happen when there is no reflector ?
    # TO DO : add reflector and rotors configuration
    # TO DO : add different versions

    ROTOR_IC = Rotor("DMTWSILRUYQNKFEJCAZBPGXOHV")
    ROTOR_IIC = Rotor("HQZGPJTMOBLNCIFDYAWVEUSRKX")
    ROTOR_IIIC = Rotor("UQNTLSZFMREHDPXKIBVYGJCWOA")
    COMMERCIAL_ENIGMA_ROTORS = [ROTOR_IC,ROTOR_IIC,ROTOR_IIIC]

    ROTOR_I = Rotor("JGDQOXUSCAMIFRVTPNEWKBLZYH")
    ROTOR_II = Rotor("NTZPSFBOKMWRCJDIVLAEYUXHGQ")
    ROTOR_III = Rotor("JVIUBHTCDYAKEQZPOSGXNRMWFL")
    UKW = Rotor("QYHOGNECVPUZTFDJAXWMKISRBL")
    ETW = Rotor("QWERTZUIOASDFGHJKPYXCVBNML")
    RAILWAY_ROTORS = [UKW,ROTOR_I,ROTOR_II,ROTOR_III]

    ROTOR_I = Rotor("EKMFLGDQVZNTOWYHXUSPAIBRCJ")
    ROTOR_II = Rotor("AJDKSIRUXBLHWTMCQGZNPYFVOE")
    ROTOR_III = Rotor("BDFHJLCPRTXVZNYEIWGAKMUSQO")
    ROTOR_IV = Rotor("ESOVPZJAYQUIRHXLNFTGKDCMWB")
    ROTOR_V = Rotor("VZBRGITYUPSDNHLXAWMJQOFECK")
    ROTOR_VI = Rotor("JPGVOUMFYQBENHZRDKASXLICTW")
    ROTOR_VII = Rotor("NZJHGRCXMYSWBOUFAIVLPEKQDT")
    ROTOR_VIII = Rotor("FKQHTLXOCBJSPDZRAMEWNIUYGV")
    ENIGMA_I_ROTORS = [ROTOR_I,ROTOR_II,ROTOR_III,ROTOR_IV,ROTOR_V,ROTOR_VI,ROTOR_VII,ROTOR_VIII]

    @obsolete
    def init_IDs(self,rotor_IDs:list) -> None:
        """TO DO"""
        max_rotor = len(self.rotors)
        self.rotor_IDs = []
        for i,ID in enumerate(rotor_IDs):
            if ID > 0 and ID <= max_rotor and not ID in rotor_IDs[(i + 1):]:
                self.rotor_IDs.append(ID)
            else:
                raise ValueError("")

    def init_positions(self,rotor_positions:list) -> None:
        """TO DO"""
        if len(rotor_positions) != len(self.rotors):
            raise ValueError("")
        for i,position in enumerate(rotor_positions):
            if type(position) == str and position >= "A" and position <= "Z":
                self.rotors[i].set_position(self.letter_to_index(position))
            elif type(position) == int and position >= 0 and position <= 25:
                self.rotors[i].set_position(position)
            else:
                raise ValueError("")

    def init_rotors(self,rotor_IDs:list) -> None:
        self.rotors = []
        for rotor_ID in rotor_IDs:
            self.rotors.append(self.available_rotors[rotor_ID])

    def init_plugboard(self,plugboard:dict[Union[str,int]]) -> None:
        """TO DO"""
        #might need to check that every value between 0 and 25 appear exactly once
        self.plugboard = {}
        for key, value in plugboard.items():
            if type(key) == str and type(value) == str and key >= "A" and key <= "Z" and value >= "A" and value <= "Z":
                self.plugboard[self.letter_to_index(key)] = self.letter_to_index(value)
            elif type(key) == int and type(value) == int and key >= 0 and key <= 25 and value >= 0 and value <= 25:
                self.plugboard[key] = value
            else:
                raise ValueError("I expected nothing from you, but i'm still disappointed")

    def init_plugboard_by_pair(self,connections:list=[]):
        """TO DO"""
        self.plugboard = {}
        for letter1, letter2 in connections:
            if type(letter1) == str and type(letter2) == str and letter1 >= "A" and letter1 <= "Z" and letter2 >= "A" and letter2 <= "Z":
                self.plugboard[self.letter_to_index(letter1)] = self.letter_to_index(letter2)
                self.plugboard[self.letter_to_index(letter2)] = self.letter_to_index(letter1)
            elif type(letter1) == int and type(letter2) == int and letter1 >= 0 and letter1 <= 25 and letter2 >= 0 and letter2 <= 25:
                self.plugboard[letter1] = letter2
                self.plugboard[letter2] = letter1
            else:
                raise ValueError("")
        # All plugs that are not connected must be linked to itself
        for i in range(26):
            if not i in self.plugboard:
                self.plugboard[i] = i

    def init_reflector(self):
        """TO DO"""
        # TO DO ; is this method useful ?
        # TO DO
        pass

    def init_settings(self,rotor_number:int,rotor_IDs:list,plugboard:dict[Union[str,int]]):
        """TO DO"""
        self.rotor_number = rotor_number
        self.init_rotors(rotor_IDs)
        self.init_plugboard(plugboard)

    def __init__(self,rotor_number:int,rotor_IDs:list,plugboard:dict[Union[str,int]]) -> None:
        """rotir_IDs is a tuple of rotor_number ints, representing the id of the rotor used
        each number in rotor_ids must be different and comprised between 1 and 8"""
        self.init_settings(rotor_number,rotor_IDs,plugboard)

    def letter_to_index(self,letter:str) -> int:
        """TO DO"""
        # TO DO : find min_str and max_str
        if letter < "A" or letter > "Z":
            raise ValueError("How can you fail such a simple task ?")
        else:
            return ord(letter) - 65
    
    def index_to_letter(self,index:int) -> str:
        """TO DO"""
        # TO DO : find min_int and max_int
        if index < 0 or index > 25:
            raise ValueError("YOU IMBECILE")
        else:
            return chr(index + 65)

    def step(self,step:int=1) -> None:
        """TO DO"""
        # TO DO : check name 
        # TO DO : handle double stepping
        # TO DO : this is incorrect : https://en.wikipedia.org/wiki/Enigma_machine#Turnover
        self.rotor_positions[0] += 1
        i = 0
        # TO DO
        while self.rotor_positions[i] > 25:
            self.rotor_positions[i] %= 26
            i += 1
            self.rotor_positions[i] += step

    def rotors_encryption(self,rotors:list,input:int) -> int:
        if rotors == []:
            return input
        else:
            return rotors[0].encrypt(self.rotors_encryption(rotors[1:],input))

    def spindle_encryption(self,input:int) -> int:
        # TO DO : self.rotors must be active rotors
        inversed_rotors = list(map(lambda rotor : rotor.inverse(),self.rotors[::-1]))
        # TO DO
        return self.rotors_encryption(inversed_rotors,self.reflector.encrypt(self.rotors_encryption(self.rotors,input)))

    def encrypt_index(self,index:int) -> int:
        """TO DO"""
        # TO DO : is the encryption supposed to be before or after the encryption ?
        # uses self.step
        # used by self.encrypt_letter
        # We have to use a variable here, because increment must be after the encryption  
        # This line is absolutely impossible to read, must improve readibility
        # This line is going to be a real nightmare to debug. And it's going to need A LOT of debug...
        #encrypted_index = self.plugboard[self.index_rotor(self.rotor3_id,self.rotor3_pos,self.index_rotor(self.rotor2_id,self.rotor2_pos,self.index_rotor(self.rotor1_id,self.rotor1_pos,self.plugboard[index])))]
        step1 = self.plugboard[index]
        step2 = self.spindle_encryption(step1)
        encrypted_index = self.plugboard[step2]
        self.step()
        return encrypted_index

    def encrypt_letter(self,letter:str) -> str:
        """TO DO
        Should be able to both encode plain text and decode crypted text"""
        # uses self.letter_to_index, self.index_to_letter, self.encrypt_index
        # used self.encrypt_message 
        # messages are entierily composed of uppercase
        index = self.letter_to_index(letter.upper())
        # TO DO : encrypt_index
        encrypted_index = self.encrypt_index(index)
        return self.index_to_letter(encrypted_index)

    '''def encrypt_message(self,text:str) -> str:
        """TO DO"""
        # recursive function
        # uses self.encrypt_letter
        if text == "":
            return ""
        else:
            letter = text[0]
            #Spaces must not be taken into account
            return self.encrypt_letter(letter) if letter != " " else "" + self.encrypt_message(text[1:])'''

    def encrypt_message(self,positions:list[Union[str,int]],message:str) -> str:
        # setting the position of the rotors
        self.init_positions(positions)
        # removing all spaces, then creating a list containing the index of all letters
        index_list = [self.letter_to_index(letter.upper()) for letter in message.replace(" ","")]
        # encrypting all indexes
        encrypted_index_list = [self.encrypt_index(index) for index in index_list]
        # converting back all indexes into letters
        encrypted_letter_list = [self.index_to_letter(index) for index in encrypted_index_list]
        # concatenating all letters by group of 4
        return " ".join(encrypted_letter_list[i:i + 4] for i in range(0,len(encrypted_letter_list),4))

if __name__ == "__main__":
    letters = [chr(i) for i in range(65, 65 + 26)]
    plugboard = {letter:letter for letter in letters}
    enigma = Enigma(3,[1,2,3],plugboard)
    print(enigma.encrypt_index(0),[0,0,0])