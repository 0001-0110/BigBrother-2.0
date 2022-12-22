using Discord;
using Discord.WebSocket;
using System.Text.RegularExpressions;

internal partial class Extensions
{
    public static List<T> Concat<T>(this List<T> list1, List<T> list2)
    {
        throw new NotImplementedException();
    }
}

internal class ChessGame
{


    internal class Position
    {
        public int X;
        public int Y;

        public Position(int x = 0, int y = 0)
        {
            X = x;
            Y = y;
        }
    }

    internal class Piece
    {
        public enum Type
        {
            Pawn,
            Knight,
            Bishop,
            Rook,
            Queen,
            King,
        }

        public enum Color
        {
            White,
            Black,
        }

        public Type type;
        public Color color;

        public Piece(Type type, Color color)
        {
            this.type = type;
            this.color = color;
        }

        public override string ToString()
        {
            // TODO Return a representation for human visualization
            return base.ToString();
        }
    }

    private bool IsPlaying;
    private Piece[] board;
    // Player ids are null until a player joins
    public ulong? Player1Id;
    public ulong? Player2Id;
    private int activeColor;

    public static Piece[] GetNewBoard()
    {
        return new Piece[] { };
    }

    public ChessGame()
    {
        IsPlaying = false;
        board = GetNewBoard();
    }

    #region PRIVATEMETHODS

    // Might want to convert all of those methods to return IEnumarable if a list is not actually useful
    private List<Position> GetPawnMoves(Position position)
    {
        throw new NotImplementedException();
    }

    private List<Position> GetKnightMoves(Position position)
    {
        throw new NotImplementedException();
    }

    private List<Position> GetDiagonalMoves(Position position)
    {
        throw new NotImplementedException();
    }

    private List<Position> GetStraightMoves(Position position)
    {
        throw new NotImplementedException();
    }

    private List<Position> GetKingMoves(Position position)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// This methods only return LEGAL moves
    /// </summary>
    /// <param name="pieceType"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private List<Position> GetMoves(Piece.Type pieceType, Position position)
    {
        return pieceType switch
        {
            Piece.Type.Pawn => GetPawnMoves(position),
            Piece.Type.Knight => GetKnightMoves(position),
            Piece.Type.Bishop => GetDiagonalMoves(position),
            Piece.Type.Rook => GetStraightMoves(position),
            Piece.Type.Queen => GetDiagonalMoves(position).Concat(GetStraightMoves(position)),
            Piece.Type.King => GetKingMoves(position),
            _ => throw new ArgumentException("This type of piece does not exist, do you even know how to play chess ?"),
        };
    }

    #endregion

    #region PUBLICMETHODS
    // This region contains all of the methods that are suppose to be called from BigBrother

    public IEnumerable<string> JoinGame(ulong playerId, bool bot = false)
    {
        if (IsPlaying)
            yield return "The game is already in progress, wait for it to finish first";

        if (Player1Id == null)
        {
            Player1Id = playerId;
            yield return $"{MentionUtils.MentionUser(playerId)} joined the game!";
        }

        if (Player1Id == playerId)
            yield return "You already joined this game";

        if (Player2Id == null)
        {
            Player2Id = playerId;
            yield return $"{MentionUtils.MentionUser(playerId)} joined the game!";
            // Two players joined, we can start the game
            yield return StartGame();
        }

        // This souldn't happen
        throw new Exception("MAYDAY MAYDAY MAYDAY");
    }

    private string StartGame()
    {
        IsPlaying = true;
        // TODO
        return "";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="move">move is under the fide notation</param>
    /// <returns></returns>
    public string TryMove(Piece.Type pieceType, Position position, Position destination)
    {
        if (!IsPlaying)
            return "Start a game before moving your pieces";

        // TODO
        return "";
    }

    public string StopGame()
    {
        Player1Id = null;
        Player2Id = null;

        return "The game has been stopped";
    }

    #endregion
}

internal partial class BigBrother
{
    private Dictionary<ulong, ChessGame> chessGames = new Dictionary<ulong, ChessGame>();

    private void InitChess()
    {
        commands.Add(new Command("chess", "` -> create or join a game of chess (WIP)", JoinChessGame));
        commands.Add(new Command("deepgreen", "` -> create a game of chess (WIP)", StartDeepGreen));
        // This string contains the regex accepting the standard fide notatio for chess moves
        // First we look at the piece that needs to move ([NBRQK])
        // Disambiguating moves is done by giving the row, or the column, or both of the tile the piece is coming from (?:([a-h])([1-8]))?
        // TODO En passant would require to add the departing column before the capture symbol, but that's too much for now
        // If the move is a capture, a x (or a semicolon) is placed between the piece and the destination ([x:])?
        // We check the destination square ([a-h])([1 - 8])
        // If there is a promotion, the player must add the name of the piece he wants to promote into ([NBRQK])?
        // This regex does not accept draw offers, as players can simply use the resign command
        // 0-0 is king size castle and 0-0-0 is queen size castle
        string fideRegex = " (?:([NBRQK])(?:([a-h])([1-8]))?([x:])?([a-h])([1-8])(?:=([NBRQ]))?)||(0-0(?:-0)?)";
        commands.Add(new Command("move", fideRegex, " <move>` -> move the piece (WIP)", Move));
        // Everyone can resign, I just don't know how they could have joined in the first place
        commands.Add(new Command("resign", "` -> resing from your current chess game", Resign, AccessLevel.Blacklist));
        commands.Add(new Command("stopchessgame", "` -> stop the current chess game (WIP)", StopChessGame, AccessLevel.Moderator));
    }

    private ChessGame? GetChessGame(IMessage message)
    {
        // TODO adapt this to make it available by private messages
        // Changing the dict from guild id to channel id might by a solution
        if (message.Channel.GetChannelType() == ChannelType.DM)
            return null;

        SocketGuild? guild = GetGuild(message.Channel);
        if (guild == null)
            throw new Exception("Guild was not found");

        // If no game exists, create a new one
        if (!chessGames.ContainsKey(guild.Id))
            chessGames[guild.Id] = new ChessGame();

        return chessGames[guild.Id];
    }

    private async Task JoinChessGame(IMessage message, GroupCollection args)
    {
        // TODO remove this when done
        await SendMessage(message.Channel, "Calm your horses, this has not been implemented yet");
        return;

        ChessGame chessGame = GetChessGame(message);

        foreach (string response in chessGame.JoinGame(message.Author.Id))
            await SendMessage(message.Channel, response);
    }

    private async Task StartDeepGreen(IMessage message, GroupCollection args)
    {
        // TODO remove this when done
        await SendMessage(message.Channel, "Slow down cowboy, this command doesn't work yet");
        return;


    }

    private async Task Move(IMessage messsage, GroupCollection args)
    {
        await SendMessage(messsage.Channel, "Stop insisting");
        return;

        // TODO
    }

    private async Task Resign(IMessage message, GroupCollection args)
    {
        await SendMessage(message.Channel, "Did you just gave up before even trying ?");
        return;

        // TODO
    }

    private async Task StopChessGame(IMessage message, GroupCollection args)
    {
        //TODO
        await SendMessage(message.Channel, "Think again");
        return;
    }
}
