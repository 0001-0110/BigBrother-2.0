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
        await SendMessage(message.Channel, "https://tenor.com/view/nileseyy-niles-disappear-vanish-peace-gif-17026388");
        await DebugLog("Quit command received");
        IsRunning = false;
    }
}