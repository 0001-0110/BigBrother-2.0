using Discord;

internal class Settings
{
    public ulong LogChannelId;

    public ActivityType? StatusType;
    public string? Status;

    public Settings(ulong logChannelId, ActivityType? statusType = null, string? status = null)
    {
        LogChannelId = logChannelId;

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
            statusType: ActivityType.Watching,
            status: "you"
            );
    }
}
