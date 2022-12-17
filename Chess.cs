using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Text.RegularExpressions;

internal class ChessGame
{
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
    public ulong Player1Id;
    public ulong Player2Id;
    private int activeColor;

    public static Piece[] GetNewBoard()
    {
        return new Piece[] { };
    }

    public ChessGame()
    {
        board = GetNewBoard();
    }

    public string StartGame(ulong player1Id, ulong player2Id)
    {
        if (IsPlaying)
            return "There already is a game in progress, wait for this one to finish";

        Player1Id = player1Id;
        Player2Id = player2Id;

        // TODO
        return "";
    }

    public string Move(string move)
    {
        if (!IsPlaying)
            return "Start a game before moving your pieces";

        // TODO
        return "";
    }
}

internal partial class BigBrother
{
    private Dictionary<ulong, ChessGame> chessGames = new Dictionary<ulong, ChessGame>();

    private void InitChess()
    {
        commands.Add(new Command("chess", "` -> create or join a game of chess (WIP)", JoinChessGame));
        commands.Add(new Command("deepgreen", "` -> create a game of chess (WIP)", StartDeepGreen));
        commands.Add(new Command("move", "", " <move>` -> move the piece (WIP)", Move));
        commands.Add(new Command("stopchessgame", "` -> stop the current chess game (WIP)", StopChessGame, AccessLevel.Moderator));
    }

    private ChessGame GetChessGame(IMessage message)
    {
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

    private async Task StopChessGame(IMessage message, GroupCollection args)
    {
        //TODO
        await SendMessage(message.Channel, "Think again");
        return;
    }
}
