﻿using System.Text.RegularExpressions;
using Services;

internal class GuildSettings
{
    public ulong? QuoteChannelId;

    public ulong[] OnDemandRoles;
    public Regex[] BannedWords;
    public string? EventsFile;

    public GuildSettings(ulong? quoteChannelId = null, ulong[]? onDemandRoles = null, Regex[]? bannedWords = null, string? eventsFile = null)
    {
        QuoteChannelId = quoteChannelId;

        OnDemandRoles = onDemandRoles ?? new ulong[0];
        BannedWords = bannedWords ?? new Regex[0];
        EventsFile = eventsFile;
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
    }

    private void LoadGuildSettings(string path)
    {
        XmlService? xmlService = XmlService.Load(path);
        if (xmlService == null)
            return;

        guildSettings[xmlService.GetValue<ulong>("GuildId")] = new GuildSettings(
            quoteChannelId: xmlService.GetValue<ulong>("QuoteChannelId"),

            // TODO Might be better to use OnDemandRoles childs
            onDemandRoles: xmlService.GetValues<ulong>("RoleId").ToArray(),
            bannedWords: xmlService.GetValues<string>("BannedWord").Select(word => new Regex(word)).ToArray(),
            eventsFile: xmlService.GetValue<string>("EventFilePath")
        );
    }
}
