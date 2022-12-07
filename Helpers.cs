using Discord;
using Discord.WebSocket;

internal partial class BigBrother
{
    private SocketGuild? GetGuild(IChannel channel)
    {
        return (channel as SocketGuildChannel)?.Guild;
    }
}
