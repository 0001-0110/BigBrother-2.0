using Discord;
using System.Text.RegularExpressions;

internal partial class BigBrother
{
    void InitSay()
    {
        commands.Add(new Command("say", " ([0-9]*) (.*)", "<channelId> <message>` -> Send the message in the given channel", Say, AccessLevel.Admin));
    }

    private async Task Say(IMessage message, GroupCollection args)
    {
        ulong channelId = ulong.Parse(args[1].Value);
        IMessageChannel? channel = client.GetChannel(channelId) as IMessageChannel;
        if (channel == null)
        {
            await SendMessage(message.Channel, "Incorrect channel ID");
        }

        await SendMessage(channelId, args[2].Value);
    }
}
