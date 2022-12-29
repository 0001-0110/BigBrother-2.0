using Discord;
using Discord.WebSocket;
using System.Text.RegularExpressions;

internal partial class BigBrother
{
    private static Regex quoteRegex = new Regex("^(?:> )|(?:[\"“«].*[\"“»])");
    private Dictionary<ulong, List<string>?> quotes = new Dictionary<ulong, List<string>?>();

    private void InitQuote()
    {
        commands.Add(new Command("reloadquotes", "` -> This is quite explicit", LoadQuotes, AccessLevel.Moderator));
        commands.Add(new Command("quote", "` -> Display a random quote from the quote channel", Quote));
    }

    private async Task LoadQuotes(IMessage message, GroupCollection args)
    {
        throw new NotImplementedException();
        //LoadQuotes
    }

    private async Task LoadQuotes(SocketGuild guild, int limit=1000)
    {
        quotes[guild.Id] = new List<string>();

        ulong? quoteChannelId = guildSettings[guild.Id].QuoteChannelId;
        // null means that this guild has no quote channel
        if (quoteChannelId == null)
            return;

        IMessageChannel? quoteChannel = client.GetChannel((ulong)quoteChannelId) as IMessageChannel;
        if (quoteChannel == null)
            return;

        IEnumerable<IMessage> messages = await quoteChannel.GetMessagesAsync(limit).FlattenAsync();
        foreach (IMessage message in messages)
            if (quoteRegex.IsMatch(message.Content))
                quotes[guild.Id]!.Add(message.Content);
    }

    private async Task Quote(IMessage message, GroupCollection args)
    {
        if (message.Channel.GetChannelType() == ChannelType.DM)
        {
            await SendMessage(message.Channel, "This command is only available on a server");
            return;
        }

        SocketGuild? guild = GetGuild(message.Channel);
        if (guild == null)
            throw new Exception("Guild not found");

        // Null only if not loaded already
        if (!quotes.ContainsKey(guild.Id))
            await LoadQuotes(guild);

        if (quotes[guild.Id]!.Count == 0)
        {
            await SendMessage(message.Channel, "Couldn't find anything worth mentioning");
            return;
        }

        await SendMessage(message.Channel, quotes[guild.Id]![random.Next(0, quotes[guild.Id]!.Count)]);
    }
}
