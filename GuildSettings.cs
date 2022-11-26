internal class GuildSettings
{
    public ulong Id;

    public Dictionary<ulong, AccessLevel>? AccessLevels;

    public bool BotChannelOnly;
    public ulong BotChannel;
    public ulong QuoteChannel;

    public string? EventsFile;

    public GuildSettings(ulong quoteChannel = 0, ulong botChannel = 0, Dictionary<ulong, AccessLevel>? accessLevels = null, string? eventFile = null)
    {
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
                    [315827580869804042] = AccessLevel.Admin,
                },
                eventFile: "Hoffnunglos_allein.csv"),
            // UNO
            //[902512847207170129] = new GuildSettings(),
        };
    }
}
