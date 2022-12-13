using System.Text.RegularExpressions;

internal class GuildSettings
{
    // If this is true, the bot can only read commands in the command channel
    // still WIP
    public bool IsBotchannelOnly;
    public ulong BotChannelId;
    public ulong QuoteChannelId;

    public Regex[] BannedWords;
    public string? EventsFile;

    public GuildSettings(ulong quoteChannelId = 0, ulong botChannelId = 0, Regex[]? bannedWords = null, string? eventFile = null)
    {
        QuoteChannelId = quoteChannelId;
        IsBotchannelOnly = botChannelId != 0;
        BotChannelId = botChannelId;

        if (bannedWords == null)
            bannedWords = new Regex[0];
        BannedWords = bannedWords;
        EventsFile = eventFile;
    }
}

internal partial class BigBrother
{
    private static Regex[] NonFunnyJokes = new Regex[]
    {
        // Feur
        new Regex("^[*. ]*[Ff.][*. ]*[Ee€3*.][*. ]*[Uu*.][*. ]*[Rr.][*. ]*"),
        // Ratio
        new Regex("^[*. ]*[Rr.][*. ]*[Aa*.][*. ]*[Tt*.][*. ]*[Ii1*.][*. ]*[Oo0*.][. ]*"),
    };

    private void InitGuildSettings()
    {
        guildSettings = new Dictionary<ulong, GuildSettings>()
        {
            // Hoffnunglos allein
            [854747950973452288] = new GuildSettings(
                quoteChannelId: 1043646501655683072,
                eventFile: "Hoffnunglos_allein.csv",
                bannedWords: NonFunnyJokes),

            // Famille
            [852593538676162570] = new GuildSettings(
                quoteChannelId: 1045083207852359710,
                botChannelId: 1045084490134982768),

            // UNO
            [902512847207170129] = new GuildSettings(
                quoteChannelId: 943492494656684103,
                bannedWords: NonFunnyJokes,
                eventFile: "UNO.csv"),

            // 22e
            [683305502763122688] = new GuildSettings(
                botChannelId: 1045046572695683072,
                eventFile: "22e.csv"),

            // Terminale Generale
            [729417290898079896] = new GuildSettings(
                quoteChannelId: 742290333848830003,
                botChannelId: 748871775005179934,
                eventFile: "Terminale_generale.csv"),
        };
    }
}
