from BLL.models.Board import *
from BLL.models.Piece import *
from BLL.models.Position import *

class BoardService:

    def __init__(self,board:Board) -> None:
        self.board = board

    def get_squares(self,rank=range(1,9),file=range(1,9)) -> Square:
        """return list of all squares"""
        squares = []
        for y in rank:
            for x in file:
                squares.append(self.board[x,y])
        return squares

    def get_pieces(self) -> Piece:
        """return list of all pieces"""
        pieces = []
        for square in self.get_squares():
            if square.has_piece():
                pieces.append(square.piece)
        return pieces

    def get_all_legal_moves(self) -> list:
        """return list of all legal moves"""
        #TO DO : this method is useless right now, we need to know the piece who's moving
        legal_moves = []
        for piece in self.get_pieces():
            if piece.color == self.board.active_color:
                #TO DO : might be better to return all moves of the same piece in the same list
                for move in self.legal_moves(piece):
                    legal_moves.append(move)
        return legal_moves

    def get_str(self):
        """return string representation of the board"""
        return "/".join("".join(str(self.board[x,y]) for x in range(1,9)) for y in range(1,9))

    def get_fen(self) -> str:
        """return fen notation of the board"""
        """
        #TO DO : maybe this method would be better in the DAL layer.
        #TO DO : might be better to use .join() to generate strings
        #TO DO : optimize this code
        #TO DO : add all other fields of the fen (active_color, castling_availibility, en_passant_target_square, halfmove_clock, fullmove_number)
        string = ""
        for y in range(8,0,-1):
            #for each line.y
            for x in range(1,9):
                #for each square
                if self[(x,y)].has_piece():
                    #this square contains a piece
                    if self[(x,y)].piece.color == 1:
                        #this piece is white
                        string += self[(x,y)].piece.notation.upper()
                    else:
                        #this piece is black
                        string += self[(x,y)].piece.notation.lower()
                else:
                    #this square is empty
                    if string[-1:].isdigit():
                        #the last character is a digit, we need to replace this last digit by +1
                        #keep all characters identical except for the last one, and increment the last character
                        #example : string = pp3 --> string = pp4
                        string = string[:-1] + str(int(string[-1:])+1)
                    else:
                        #the last character is not a digit, we need to add 1
                        string += "1"
            #end of this rank
            string += "/"
        #remove the last slash
        return string[:-2]
        """
        #Piece placement :
        piece_placement = self.get_str()
        index = 0
        while index < len(piece_placement):
            if piece_placement[index] == ".":
                empty_squares = 1
                while piece_placement[index + empty_squares] == ".":
                    empty_squares += 1
                piece_placement = piece_placement[:index]+ str(empty_squares) + piece_placement[index + empty_squares:]
            index += 1

        #Active color :
        active_color = "w" if self.board.active_color == 1 else "b"

        #Castling ability :
        #TO DO :
        castling_ability = None

        #En passant target square :
        en_passant_target_square = self.board.en_passant_target_square

        #halfmove_clock :
        halfmove_clock = self.board.halfmove_clock

        #fullmove_number :
        fullmove_number = self.board.fullmove_number

        return " ".join([piece_placement,active_color,castling_ability,en_passant_target_square,halfmove_clock,fullmove_number])
                
    def is_in_board(self,position) -> bool:
        """return True if position is a valid position on thios board, else return False"""
        #TO DO : replace all the raw values 
        if type(position) == Position:
            return position.x <= 8 and position.x >= 1 and position.y <= 8 and position.y >= 1
        elif type(position) == tuple:
            return position[0] <= 8 and position[0] >= 1 and position[1] <= 8 and position[1] >= 1

    def init_board(self,variant=0) -> None:
        """return None, reset the board with each pieces at his original place"""
        #init all square
        #TO DO : variant 
        for x in range(1,9):
            for y in range(1,9):
                self.board[(x,y)] = Square(Position(x,y))
        #init all pieces
        self.load_board("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")

    def load_board(self,fen:str) -> None:
        """fill the board with pieces following the given fen string
        A FEN record contains six fields. The separator between fields is a space. The fields are:
        0. Piece placement (from White's perspective). Each rank is described, starting with rank 8 and ending with rank 1; within each rank, the contents of each square are described from file "a" through file "h". Following the Standard Algebraic Notation (SAN), each piece is identified by a single letter taken from the standard English names (pawn = "P", knight = "N", bishop = "B", rook = "R", queen = "Q" and king = "K"). White pieces are designated using upper-case letters ("PNBRQK") while black pieces use lowercase ("pnbrqk"). Empty squares are noted using digits 1 through 8 (the number of empty squares), and "/" separates ranks.
        1. Active color. "w" means White moves next, "b" means Black moves next.
        2. Castling availability. If neither side can castle, this is "-". Otherwise, this has one or more letters: "K" (White can castle kingside), "Q" (White can castle queenside), "k" (Black can castle kingside), and/or "q" (Black can castle queenside). A move that temporarily prevents castling does not negate this notation.
        3. En passant target square in algebraic notation. If there's no en passant target square, this is "-". If a pawn has just made a two-square move, this is the position "behind" the pawn. This is recorded regardless of whether there is a pawn in position to make an en passant capture.[6]
        4. Halfmove clock: This is the number of halfmoves since the last capture or pawn advance. The reason for this field is that the value is used in the fifty-move rule.
        5. Fullmove number: The number of the full move. It starts at 1, and is incremented after Black's move."""
        #TO DO : maybe this method would be better in the DAL layer.
        #TO DO : what happens if impossible fen are given ?
        
        piece_placement, active_color, castling_availibility, en_passant_target_square, halfmove_clock, fullmove_number = fen.split(" ")
        
        #fen always begin from the top left corner
        x, y = 1, 8
        for character in piece_placement:
            if character.isdigit():
                #empty spaces
                #-1 is because the active square is already took into account
                x += int(character)
            elif character == "/":
                #next rank
                x = 1
                y -= 1      #TO DO : need to check if still in board
            elif character.isalpha():
                #this character is a letter
                if character.isupper():
                    #this character is capitalize, so white piece
                    color = 1
                else:
                    #else is black
                    color = -1
                for piece_type in piece_types:
                    if piece_type.notation == character.lower():
                        self.board[(x,y)].set_piece(piece_type(color,Position(x,y)))
                        #print(self[(x,y)].piece)
                #next square
                x += 1

        #set active color. "w" means white and "b" means black
        self.board.active_color = 1 if active_color == "w" else -1

        #TO DO : castling_availibility

        #ord -96 is to convert line(alpha) into x(int)
        if en_passant_target_square == "-":
            self.board.en_passant_target_square = None
        else:
            self.board.en_passant_target_square = Position(ord(en_passant_target_square[0])-96,int(en_passant_target_square[1]))

        self.board.halfmove_clock = halfmove_clock

        self.board.fullmove_number = fullmove_number

    def move_piece(self,from_position:Position,to_position:Position) -> None:
        if not (self.is_in_board(from_position) and self.is_in_board(to_position)):
            #can't move pieces outside of board
            #TO DO : raise error
            pass
        elif self.board[from_position].piece == None:
            #no piece on the original square
            #TO DO : raise error
            pass
        #TO DO : create legal_moves function
        #elif not to_position in self.board[from_position].piece.legal_moves():
            #this piece cannot move to this square
            #TO DO : raise error
            #pass
        else:
            #this move is legal
            #TO DO : need to add check verification
            #TO DO : handle captures
            self.board[to_position].piece = self[from_position].piece
            self.board[from_position].piece = None
            self.board[to_position].piece.position = to_position

    def pawn_legal_moves(self,piece:Pawn) -> list:
        legal_moves = []

        #pawn can go forward, the direction they are heading is depending on their color
        forward_move = Position(0,piece.color)
        if self.is_in_board(piece.position + forward_move):
            if not self.board[piece.position + forward_move].has_piece():
                #if the pawn is blocked, it cannot go forward
                legal_moves.append(forward_move)

        #on the first move, pawn can go forward from two squares
        double_forward_move = Position(0,2*piece.color)
        if piece.has_moved == False:
            #this piece has never moved
            if self.is_in_board(piece.position + forward_move):
                if not self.board[piece.position + forward_move].has_piece():
                    legal_moves.append(double_forward_move)

        #pawn can captures
        captures_moves = [Position(-1,piece.color),Position(1,piece.color)]
        for move in captures_moves:
            if self.is_in_board(piece.position + move):
                if self.board[piece.position + move].has_piece(-piece.color):
                    #this square contains an enemy piece
                    legal_moves.append(move)

        #TO DO : add en passant moves

        return legal_moves

    def knight_legal_moves(self,piece:Knight) -> list:
        legal_moves = []
        moves = [
            Position(-2,-1),
            Position(-2,1),
            Position(-1,2),
            Position(1,2),
            Position(2,1),
            Position(2,-1),
            Position(1,-2),
            Position(-1,-2)
            ]
        for move in moves:
            if self.is_in_board(piece.position + move):
                if not self.board[piece.position + move].has_piece(piece.color):
                    #cannot jump to a square if this square contains a freindly piece
                    legal_moves.append(move)
        return legal_moves

    def bishop_legal_moves(self,piece:Piece) -> list:
        """return list"""
        #TO DO : add comments on this code
        legal_moves = []
        for horizontal_direction in [-1,1]:
            for vertical_direction in [-1,1]:
                for i in range(1,8):
                    #TO DO : need variable change
                    move = Position(i*horizontal_direction,i*vertical_direction)
                    #TO DO : this line might cause errors because self[] could be called even if this square is not in the board. Need to check if the and works correctly
                    if self.is_in_board(piece.position + move) and not self.board[piece.position + move].has_piece(piece.color):
                        legal_moves.append(move)
                        if self.board[(piece.position + move)].has_piece():
                            break
                    else:
                        #if this square is blocked, this piece cannot jump over it, and thus, there is no need to check on the next square
                        break
        return legal_moves

    def rook_legal_moves(self,piece:Piece) -> list:
        """return a list"""
        #TO DO : might need name change, this one is not clear enough
        #TO DO : add comments on this code
        legal_moves = []
        for horizontal_direction in [-1,1]:
            #check both sides
            for x in range(1,8):
                #range of 7 is because the board is 8 square, so no pieces can travel more than 7 squares
                move = Position(x*horizontal_direction,0)
                if self.is_in_board(piece.position + move) and not self.board[piece.position + move].has_piece(piece.color):
                    legal_moves.append(move)
                    if self.board[piece.position + move].has_piece():
                        break
                else:
                    #if this square is blocked, this piece cannot jump over it, and thus, there is no need to check on the next square
                    break
        for vertical_direction in [-1,1]:
            #check both sides
            for y in range(1,8):
                #range of 7 is because the board is 8 square, so no pieces can travel more than 7 squares
                move = Position(0,y*vertical_direction)
                if self.is_in_board(piece.position + move) and not self.board[piece.position + move].has_piece(piece.color):
                    legal_moves.append(move)
                    if self.board[piece.position + move].has_piece():
                        break
                else:
                    #if this square is blocked, this piece cannot jump over it, and thus, there is no need to check on the next square
                    break
        return legal_moves
        
    def king_legal_moves(self,piece:King) -> list:
        #TO DO : this method is creating index errors while
        legal_moves = []
        for x in range(-1,2):
            for y in range(-1,2):
                move = Position(x,y)
                if not (x == 0 and y == 0):
                    if self.is_in_board(piece.position + move):
                        if not self.board[piece.position + move].has_piece(piece.color):
                            legal_moves.append(move)
        return legal_moves

    def legal_moves(self,piece) -> list:
        """return all legal moves from the piece on [position] square"""
        #TO DO : handle checks
        if type(piece) == Pawn:
            return self.pawn_legal_moves(piece)
        elif type(piece) == Knight:
            return self.knight_legal_moves(piece)
        elif type(piece) == Bishop:
            return self.bishop_legal_moves(piece)
        elif type(piece) == Rook:
            return self.rook_legal_moves(piece) 
        elif type(piece) == Queen:
            return self.bishop_legal_moves(piece) + self.rook_legal_moves(piece) 
        elif type(piece) == King:
            return self.king_legal_moves(piece)

    def is_check(self) -> bool:
        """return True if the king is in check"""
        #check only for the active king, as the other one can't be in check (game would be over)
        pass

    def is_checkmate(self) -> bool:
        """return True if the king is in checkmate"""
        #check only for the active king, as the other one can't be checkmated (game would be over)
        pass

    def is_draw(self) -> bool:
        """return True if the game is a draw"""
        pass

    def piece_value(self,piece) -> int:
        """return value of this piece only"""
        #first, we add the value of the piece itself
        value = piece.value
        for legal_move in piece.legal_moves:
            if self[legal_move].has_piece():
                if self[legal_move].piece.color == piece.color:
                    #you are protecting your own piece
                    value += 1000 - self[legal_move].piece.value
                else:
                    #you are threatening the opposant piece
                    value += self[legal_move].piece.value
            else:
                #you are covering an empty square
                value += 10

    def estimate_board_value(self) -> int:
        """return the value of the board. positive means advantagous for white, negative means advantagous for black."""
        #TO DO : this method is incomplete
        for piece in self.get_pieces():
            pass