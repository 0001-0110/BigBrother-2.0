internal class GuildSettings
{
    public Dictionary<ulong, AccessLevel> AccessLevels;

    public bool BotChannelOnly;
    public ulong? BotChannel;
    public ulong? QuoteChannel;

    public string? EventsFile;

    public GuildSettings(ulong? quoteChannel = null, ulong? botChannel = null, Dictionary<ulong, AccessLevel>? accessLevels = null, string? eventFile = null)
    {
        if (accessLevels == null)
            accessLevels = new Dictionary<ulong, AccessLevel>();
        AccessLevels = accessLevels;

        QuoteChannel = quoteChannel;
        BotChannelOnly = botChannel != 0;
        BotChannel = botChannel;

        EventsFile = eventFile;
    }
}

internal partial class BigBrother
{
    private void InitGuildSettings()
    {
        guildSettings = new Dictionary<ulong, GuildSettings>()
        {
            // Hoffnunglos allein
            [854747950973452288] = new GuildSettings(
                quoteChannel: 1043646501655683072,
                accessLevels: new Dictionary<ulong, AccessLevel>()
                {
                    // 22
                    [315827580869804042] = AccessLevel.Admin,
                },
                eventFile: "Hoffnunglos_allein.csv"),

            // Famille
            [852593538676162570] = new GuildSettings(
                quoteChannel: 1045083207852359710,
                botChannel: 1045084490134982768,
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
                quoteChannel: 943492494656684103,
                accessLevels: new Dictionary<ulong, AccessLevel>()
                {
                    // 22
                    [315827580869804042] = AccessLevel.Admin,
                    // Magali
                    [284210389070381057] = AccessLevel.Moderator,
                },
                eventFile: "UNO.csv"),

            // 22e
            [683305502763122688] = new GuildSettings(
                botChannel: 1045046572695683072,
                accessLevels: new Dictionary<ulong, AccessLevel>()
                {
                    // 22
                    [315827580869804042] = AccessLevel.Admin,
                    // Barbarus
                    [535429170864586754] = AccessLevel.Admin,
                    // Barbar
                    [641741839472263176] = AccessLevel.Moderator,
                },
                eventFile:"22e.csv"),

            // Terminale Generale
            [729417290898079896] = new GuildSettings(
                quoteChannel: 742290333848830003,
                botChannel: 748871775005179934,
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
