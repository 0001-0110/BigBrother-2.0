using Discord;
using Discord.WebSocket;
using System.Text.RegularExpressions;

internal class ChessGame
{

}

internal partial class BigBrother
{
    private Dictionary<ulong, ChessGame> chessGames = new Dictionary<ulong, ChessGame>();

    private void InitChess()
    {
        commands.Add(new Command("chess", "` -> (WIP)", JoinChessGame));
        commands.Add(new Command("deepgreen", "` -> (WIP)", StartDeepGreen));
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
    }

    private async Task StartDeepGreen(IMessage message, GroupCollection args)
    {
        // TODO remove this when done
        await SendMessage(message.Channel, "Slow down cowboy, this command doesn't work yet");
    }
}
