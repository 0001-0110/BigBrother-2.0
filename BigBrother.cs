using Discord;
using Discord.WebSocket;

internal partial class BigBrother
{
    private string tokenFile;

    private bool IsRunning;
    private DiscordSocketClient client;
    private Settings settings;
    Dictionary<ulong, GuildSettings> guildSettings;

    private IMessageChannel? logChannel;

    private List<Command> commands;

    private Random random = new Random();

    public BigBrother(string tokenFile)
    {
        this.tokenFile = tokenFile;

        client = new DiscordSocketClient(
            new DiscordSocketConfig() { GatewayIntents = GatewayIntents.All });
        client.Log += Log;
        client.Ready += ClientReady;
        client.MessageReceived += MessageReceived;

        commands = new List<Command>();

        InitSettings();
        InitGuildSettings();

        InitDice();
        InitQuote();
        InitBattle();
        InitHangman();
    }

    public async Task Run()
    {
        IsRunning = true;

        await Connect();

        while (IsRunning)
            await Task.Delay(5000);
        await client.LogoutAsync();
        await client.StopAsync();
    }

    private async Task Log(LogMessage log)
    {
        await SendMessage(logChannel, $"```cs\n{log}```");
    }

    private async Task Connect()
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
