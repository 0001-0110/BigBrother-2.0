using System.Text.RegularExpressions;

internal class GuildSettings
{
    public Dictionary<ulong, AccessLevel> AccessLevels;

    // If this is true, the bot can only read commands in the command channel
    // still WIP
    public bool IsBotchannelOnly;
    public ulong BotChannelId;
    public ulong QuoteChannelId;

    public Regex[] BannedWords;
    public string? EventsFile;

    public GuildSettings(ulong quoteChannelId = 0, ulong botChannelId = 0, Dictionary<ulong, AccessLevel>? accessLevels = null, Regex[]? bannedWords = null, string? eventFile = null)
    {
        if (accessLevels == null)
            accessLevels = new Dictionary<ulong, AccessLevel>();
        AccessLevels = accessLevels;

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
        new Regex("[*. ]*[Ff*.][*. ]*[Ee€3*.][*. ]*[Uu*.][*. ]*[Rr*.][*. ]*"),
        // Ratio
        new Regex("[*. ]*[Rr*.][*. ]*[Aa*.][*. ]*[Tt*.][*. ]*[Ii1*.][*. ]*[Oo0*.][*. ]*"),
    };

    private void InitGuildSettings()
    {
        guildSettings = new Dictionary<ulong, GuildSettings>()
        {
            // Hoffnunglos allein
            [854747950973452288] = new GuildSettings(
                quoteChannelId: 1043646501655683072,
                accessLevels: new Dictionary<ulong, AccessLevel>()
                {
                    // 22
                    [315827580869804042] = AccessLevel.Admin,
                },
                eventFile: "Hoffnunglos_allein.csv"),

            // Famille
            [852593538676162570] = new GuildSettings(
                quoteChannelId: 1045083207852359710,
                botChannelId: 1045084490134982768,
                accessLevels: new Dictionary<ulong, AccessLevel>()
                {
                    // 22
                    [315827580869804042] = AccessLevel.Admin,
                    // Technoprof
                    [518431338198728714] = AccessLevel.Admin,
                    // lexou
                    [315842702753398785] = AccessLevel.Admin,
                }),

            // UNO
            [902512847207170129] = new GuildSettings(
                quoteChannelId: 943492494656684103,
                accessLevels: new Dictionary<ulong, AccessLevel>()
                {
                    // 22
                    [315827580869804042] = AccessLevel.Admin,
                    // Magali
                    [284210389070381057] = AccessLevel.Moderator,
                },
                bannedWords: NonFunnyJokes,
                eventFile: "UNO.csv"),

            // 22e
            [683305502763122688] = new GuildSettings(
                botChannelId: 1045046572695683072,
                accessLevels: new Dictionary<ulong, AccessLevel>()
                {
                    // 22
                    [315827580869804042] = AccessLevel.Admin,
                    // Barbarus
                    [535429170864586754] = AccessLevel.Admin,
                    // Barbar
                    [641741839472263176] = AccessLevel.Moderator,
                },
                eventFile: "22e.csv"),

            // Terminale Generale
            [729417290898079896] = new GuildSettings(
                quoteChannelId: 742290333848830003,
                botChannelId: 748871775005179934,
                accessLevels: new Dictionary<ulong, AccessLevel>()
                {
                    // 22
                    [315827580869804042] = AccessLevel.Admin,
                    // Barbarus
                    [535429170864586754] = AccessLevel.Admin,
                },
                eventFile: "Terminale_generale.csv"),
        };
    }
}
