using System.Text.RegularExpressions;
using System.Xml.Linq;

internal class GuildSettings
{
    public ulong? QuoteChannelId;

    public ulong[] OnDemandRoles;
    public Regex[] BannedWords;
    public string? EventsFile;

    //public GuildSettings() { }

    public GuildSettings(ulong? quoteChannelId = null, ulong[]? onDemandRoles = null, Regex[]? bannedWords = null, string? eventFile = null)
    {
        QuoteChannelId = quoteChannelId;

        OnDemandRoles = onDemandRoles ?? new ulong[0];
        BannedWords = bannedWords ?? new Regex[0];
        EventsFile = eventFile;
    }
}

internal partial class BigBrother
{
    private const string GUILDSETTINGSFOLDERPATH = "Settings\\GuildSettings";

    private static Regex[] NonFunnyJokes = new Regex[]
    {
        // Feur
        new Regex("^[*. ]*[Ff.][*. ]*[Ee€3*.][*. ]*[Uu*.][*. ]*[Rr.][*. ]*"),
        // Ratio
        new Regex("^[*. ]*[Rr.][*. ]*[Aa*.][*. ]*[Tt*.][*. ]*[Ii1*.][*. ]*[Oo0*.][. ]*"),
    };

    private void InitGuildSettings()
    {
        // TODO put all of this in a file
        guildSettings = new Dictionary<ulong, GuildSettings>();
        foreach (string path in Directory.GetFiles(GetPath(GUILDSETTINGSFOLDERPATH)))
            LoadGuildSettings(path);

        DebugLog("InitGuildSettings done");

        /*guildSettings = new Dictionary<ulong, GuildSettings>()
        {
            // Hoffnunglos allein
            [854747950973452288] = new GuildSettings(
                quoteChannelId: 1043646501655683072,
                onDemandRoles: new ulong[]
                {
                    // test role
                    1055154370905378847,
                },
                eventFile: "Hoffnunglos_allein.csv",
                bannedWords: NonFunnyJokes),

            // Famille
            [852593538676162570] = new GuildSettings(
                quoteChannelId: 1045083207852359710),

            // UNO
            [902512847207170129] = new GuildSettings(
                quoteChannelId: 943492494656684103,
                bannedWords: NonFunnyJokes,
                eventFile: "UNO.csv"),

            // Zagreb
            [999788857392378008] = new GuildSettings(
                quoteChannelId: 1043455400151896175
                ),

            // 22e Division
            [683305502763122688] = new GuildSettings(
                onDemandRoles: new ulong[]
                {
                    // Team Fortress 2
                    907637378246643782,
                    // War thunder
                    906532152072622111,
                    // Rainbow 6
                    894679616881586197,
                    // Sea of thieves
                    906532281596936212,
                    // Company of heroes
                    908391645555347476,
                    // Post Scriptum
                    907637525466734652,
                    // Civilization
                    907637063166361600,
                    // Kerbal Space Program
                    908311272863432705,
                    // Age of empire
                    908681308631363604,
                    // Among us
                    912068625656057910,
                },
                eventFile: "22e.csv"),

            // Terminale Generale
            [729417290898079896] = new GuildSettings(
                quoteChannelId: 742290333848830003,
                eventFile: "Terminale_generale.csv"),
        };*/
    }

    private async void LoadGuildSettings(string path)
    {
        XmlService? xmlService = XmlService.Load(path);
        if (xmlService == null)
            return;

        guildSettings[xmlService.GetValue<ulong>("GuildId")] = new GuildSettings()
        {
            QuoteChannelId = xmlService.GetValue<ulong>("QuoteChannelId"),

            OnDemandRoles = xmlService.GetValues<ulong>("RoleId").ToArray(),
            BannedWords = xmlService.GetValues<string>("BannedWord").Select(word => new Regex(word)).ToArray(),
            EventsFile = xmlService.GetValue<string>("EventFilePath"),
        };
    }
}
