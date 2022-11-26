﻿using Discord;
using Discord.WebSocket;
using System.Text.RegularExpressions;

internal partial class BigBrother
{
    private static Regex replaceIds = new Regex("{([1-9]*)}");

    private async Task SendMessage(ulong channelId, string message, bool isTTS=false)
    {
        IMessageChannel? channel = client.GetChannel(channelId) as IMessageChannel;
        await SendMessage(channel, message, isTTS);
    }

    private async Task SendMessage(IMessageChannel? channel, string message, bool isTTS=false)
    {
        if (channel == null)
            return;

        Match match = replaceIds.Match(message);
        for (int i = 0; i < match.Groups.Count; i++)
        {
            var idString = match.Groups[i].Value;
            ulong id;
            if (ulong.TryParse(idString, out id))
                message = replaceIds.Replace(idString, MentionUtils.MentionUser(id));
        }
        await channel.SendMessageAsync(message, isTTS);
    }

    private async Task DebugLog(string logMessage)
    {
        await SendMessage(logChannel, $"```\n{logMessage}```");
    }

    private async Task DeleteMessage(IMessage message)
    {
        await message.DeleteAsync();
    }

    private async Task MessageReceived(SocketMessage messageReceived)
    {
        // To avoid confusion when multiple instances of this function are runnning at the same time
        SocketMessage message = messageReceived;
        Console.WriteLine($"message: {message.Content}");

        // Ignore its own messages
        if (message.Author.Id == client.CurrentUser.Id)
            return;

        if (message.Content.StartsWith(Command.Prefix))
        {
            await CommandReceived(message);
            return;
        }
    }
}
