using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;

internal partial class BigBrother
{
    private const string OPENAIAPIKEYFILE = "OpenAIAPIKey.txt";
    private string openAIKey;

    private void InitMessage()
    {
        // TODO handle that safely
        using (StreamReader streamReader = new StreamReader(GetPath(OPENAIAPIKEYFILE)))
            openAIKey = streamReader.ReadToEnd();

        commands.Add(new Command("say", " ([0-9]+) (.*)", " <channelId> <message>` -> Send the message in the given channel", Say, AccessLevel.Admin));
    }

    private async Task<IUserMessage?> SendMessage(ulong channelId, string message, bool isTTS = false)
    {
        IMessageChannel? channel = client.GetChannel(channelId) as IMessageChannel;
        return await SendMessage(channel, message, isTTS);
    }

    private async Task<IUserMessage?> SendMessage(IMessageChannel? channel, string message, bool isTTS = false)
    {
        if (channel == null)
            return null;

        return await channel.SendMessageAsync(message, isTTS);
    }

    private async Task DeleteMessage(IMessage message)
    {
        DebugLog($"Deleting:\n> {message.Content}\n from {message.Author}");
        await message.DeleteAsync();
    }

    private bool IsBannedWord(SocketMessage message)
    {
        if (message.Channel.GetChannelType() == ChannelType.DM)
            return false;

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
        // If the message is private, answer too
        if (message.MentionedUsers.Any(user => user.Id == client.CurrentUser.Id) || message.Channel.GetChannelType() == ChannelType.DM)
        {
            // This feature is only available until April 1st 2023
            if (DateTime.Now > new DateTime(2023, 04, 01))
            {
                await SendMessage(message.Channel, "This feature is no longer available\nUse this instead: https://chat.openai.com/chat");
                return;
            }

            using (var client = new HttpClient())
            {
                IDisposable typing = message.Channel.EnterTypingState();

                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {openAIKey}");

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

                typing.Dispose();
            }
            return;
        }
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
