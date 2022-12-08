using Discord;
using System.Text.RegularExpressions;

internal partial class BigBrother
{
    void InitSay()
    {
        commands.Add(new Command("say", " ([0-9]+) (.*)", "<channelId> <message>` -> Send the message in the given channel", Say, AccessLevel.Admin));
    }

    private async Task Say(IMessage message, GroupCollection args)
    {
        ulong channelId;
        if (!ulong.TryParse(args[1].Value, out channelId))
        {
            await SendMessage(message.Channel, "Incorrect channel ID");
            return;
        }

        IMessageChannel? channel = client.GetChannel(channelId) as IMessageChannel;
        if (channel == null)
        {
            await SendMessage(message.Channel, "Incorrect channel ID");
            return;
        }

        await SendMessage(channelId, args[2].Value);
    }
}
