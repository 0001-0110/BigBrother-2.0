using Discord;
using Discord.WebSocket;

internal partial class BigBrother
{
    private const string TOKENFILE = "token.txt";
    private string localPath;
    
    private bool IsRunning;
    private bool IsReady;
    private DiscordSocketClient client;
    private Action onReady;
    private Settings settings;
    Dictionary<ulong, GuildSettings> guildSettings;

    private IMessageChannel? logChannel;

    private Random random = new Random();

    public BigBrother(string localPath)
    {
        IsRunning = false;
        IsReady = false;

        this.localPath = localPath;

        client = new DiscordSocketClient(
            new DiscordSocketConfig() { GatewayIntents = GatewayIntents.All });
        client.Log += Log;
        client.Ready += ClientReady;
        client.Disconnected += Disconnected;
        client.MessageReceived += MessageReceived;

        commands = new List<Command>();

        InitSettings();
        InitGuildSettings();

        InitAllCommands();
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
        using (StreamReader file = new StreamReader(GetPath(TOKENFILE)))
        {
            string token = file.ReadToEnd();
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();
        }
    }

    private async Task ClientReady()
    {
        IsReady = true;

        logChannel = await client.GetChannelAsync(settings.LogChannelId) as IMessageChannel;

        await client.SetStatusAsync(UserStatus.Online);
        if (settings.StatusType != null)
            await client.SetGameAsync(settings.Status, type: (ActivityType)settings.StatusType);

        onReady.Invoke();
    }

    private async Task Disconnect()
    {
        IsRunning = false;
        await client.StopAsync();
        await client.LogoutAsync();
    }

    private async Task Disconnected(Exception exception)
    {
        await client.SetStatusAsync(UserStatus.Offline);

        IsReady = false;
    }
}
