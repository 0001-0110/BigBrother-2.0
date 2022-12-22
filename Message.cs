using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;

internal partial class BigBrother
{
    private async Task SendMessage(ulong channelId, string message, bool isTTS=false)
    {
        IMessageChannel? channel = client.GetChannel(channelId) as IMessageChannel;
        await SendMessage(channel, message, isTTS);
    }

    private async Task SendMessage(IMessageChannel? channel, string message, bool isTTS=false)
    {
        if (channel == null)
            return;

        await channel.SendMessageAsync(message, isTTS);
    }

    private async Task DebugLog(string logMessage)
    {
        await SendMessage(logChannel, $"```\n{logMessage}```");
    }

    private async Task DeleteMessage(IMessage message)
    {
        await DebugLog($"Deleting:\n> {message.Content}\n from {message.Author}");
        await message.DeleteAsync();
    }

    private bool IsBannedWord(SocketMessage message)
    {
        GuildSettings activeGuildSettings = GetGuildSettings(message.Channel);
        foreach (Regex bannedWord in activeGuildSettings.BannedWords)
            if (bannedWord.IsMatch(message.Content))
                return true;
        return false;
    }

    private async Task MessageReceived(SocketMessage messageReceived)
    {
        // To avoid confusion when multiple instances of this function are runnning at the same time
        SocketMessage message = messageReceived;

        // Ignore its own messages
        if (message.Author.Id == client.CurrentUser.Id)
            return;

        if (message.Content.StartsWith(Command.Prefix))
        {
            await CommandReceived(message);
            return;
        }

        if (IsBannedWord(message))
        {
            await DeleteMessage(message);
            return;
        }

        // If the bot is mentioned, uses chatGPT to answer
        if (message.MentionedUsers.Any(user => user.Id == client.CurrentUser.Id))
        {
            // TODO hide this in a file
            string apiKey = "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";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                var request = new
                {
                    model = "text-davinci-003",
                    prompt = $"{message.Content}\n",
                    max_tokens = 2048,
                    // I want Big Brother to be creative
                    temperature = 0.9
                };

                var response = await client.PostAsync($"https://api.openai.com/v1/completions",
                    new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    dynamic data = JsonConvert.DeserializeObject(json);
                    string answer = data.choices[0].text;

                    // TODO adapt this for DMs
                    //IGuild guild = GetGuild(message.Channel);
                    //if (answer.Contains(guild.EveryoneRole.Mention))
                    await SendMessage(message.Channel, answer);
                }
            }
        }
    }
}
