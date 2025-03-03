using Discord;

internal class Settings
{
    public ulong LogChannelId;

    public Dictionary<ulong, AccessLevel> AccessLevels;

    public ActivityType? StatusType;
    public string? Status;

    public Settings(ulong logChannelId, Dictionary<ulong, AccessLevel>? accessLevels = null, ActivityType? statusType = null, string? status = null)
    {
        LogChannelId = logChannelId;

        if (accessLevels == null)
            accessLevels = new Dictionary<ulong, AccessLevel>();
        AccessLevels = accessLevels;

        StatusType = statusType;
        Status = status;
    }
}

internal partial class BigBrother
{
    private void InitSettings()
    {
        settings = new Settings(
            logChannelId: 1044351592515768431,
            accessLevels: new Dictionary<ulong, AccessLevel>()
            {
                // 22
                [315827580869804042] = AccessLevel.Admin,
                // Technoprof
                [518431338198728714] = AccessLevel.Admin,
                // lexou
                [315842702753398785] = AccessLevel.Admin,
                // Magali
                [284210389070381057] = AccessLevel.Moderator,
                // Barbarus
                [535429170864586754] = AccessLevel.Moderator,
                // Barbar
                [641741839472263176] = AccessLevel.Moderator,
            },
            statusType: ActivityType.Watching,
            status: "you (2.0)"
            );
    }
}
