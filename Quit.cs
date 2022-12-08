using Discord;
using System.Text.RegularExpressions;

internal partial class BigBrother
{
    private void InitQuit()
    {
        commands.Add(new Command("quit", "` -> Don't touch that", Quit, AccessLevel.Moderator));
    }

    private async Task Quit(IMessage message, GroupCollection args)
    {
        await SendMessage(message.Channel, "Quitting! It's like trying, but easier");
        await DebugLog("Quit command received");
        await client.SetGameAsync("dead", type: ActivityType.Playing);
        IsRunning = false;
    }
}
