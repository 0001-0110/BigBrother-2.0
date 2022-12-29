using Discord;
using Discord.WebSocket;

internal partial class BigBrother
{
    public static BigBrother Instance;

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
        Instance = this;

        IsRunning = false;
        IsReady = false;

        this.localPath = localPath;

        client = new DiscordSocketClient(
            new DiscordSocketConfig() { GatewayIntents = GatewayIntents.All });
        client.Log += Log;
        client.Ready += ClientReady;
        client.MessageReceived += MessageReceived;

        commands = new List<Command>();

        InitDebug();
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

        await Disconnect();
    }

    private async Task Log(LogMessage log)
    {
        await Task.Yield();
        DebugLog(log.ToString());
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
        await client.SetStatusAsync(UserStatus.Online);
        if (settings.StatusType != null)
            await client.SetGameAsync(settings.Status, type: (ActivityType)settings.StatusType);

        onReady.Invoke();

        IsReady = true;
    }

    private async Task Disconnect()
    {
        await client.SetStatusAsync(UserStatus.Offline);
        IsReady = false;
        IsRunning = false;
        await client.StopAsync();
        await client.LogoutAsync();
    }
}
