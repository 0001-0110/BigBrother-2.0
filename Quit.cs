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
        await SendMessage(message.Channel, "Let's call it a day!");
        await DebugLog("Quit command received");
        await Disconnect();
    }
}
