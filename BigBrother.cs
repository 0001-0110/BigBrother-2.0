using Discord;
using Discord.WebSocket;

internal partial class BigBrother
{
    private bool IsRunning;
    private DiscordSocketClient client;
    private Settings settings;
    Dictionary<ulong, GuildSettings> guildSettings;

    private IMessageChannel? logChannel;

    public BigBrother()
    {
        client = new DiscordSocketClient();
        client.Log += Log;
        client.Ready += ClientReady;
        // TODO
        //client.MessageReceived += 

        InitSettings();
        InitGuildSettings();

        InitBattle();
        InitHangman();
    }

    public async Task Run(string tokenFile)
    {
        IsRunning = true;

        await Connect(tokenFile);

        while (IsRunning)
            await Task.Delay(5000);
    }

    private async Task Log(LogMessage log)
    {
        await SendMessage(logChannel, log.ToString());
    }

    private async Task Connect(string tokenFile)
    {
        string token;
        using (StreamReader file = new StreamReader(tokenFile))
            token = file.ReadToEnd();
        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();
    }

    private async Task ClientReady()
    {
        logChannel = await client.GetChannelAsync(settings.LogChannelId) as IMessageChannel;

        if (settings.StatusType != null)
            await client.SetGameAsync(settings.Status, type: (ActivityType)settings.StatusType);
    }
}
