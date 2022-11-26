using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Text.RegularExpressions;

enum AccessLevel
{
    Blacklist = -2,
    Limited = -1,
    Standard = 0,
    Admin = 1,
    Moderator = 2,
}

internal class Command
{
    public readonly static char Prefix = '!';
    public readonly static string ArgPrefix = "--";

    public string Name;
    private Regex commandRegex;
    private Regex commandRegexArgs;
    private Func<IMessage, GroupCollection, Task> execute;
    public AccessLevel AccessLevel;

    public Command(string name, Func<IMessage, GroupCollection, Task> function, AccessLevel accessLevel = AccessLevel.Standard)
        : this(name, "", function, accessLevel) { }

    public Command(string name, string argsRegex, Func<IMessage, GroupCollection, Task> function, AccessLevel accessLevel = AccessLevel.Standard)
    {
        Name = name;
        string prefixedCommand = $"^{Prefix}{name}";
        commandRegex = new Regex(prefixedCommand);
        if (argsRegex.Length == 0)
            commandRegexArgs = new Regex($"{prefixedCommand}$");
        else
            // " ?" between the command and the args to account for the space only if there is given args
            commandRegexArgs = new Regex($"{prefixedCommand} ?{argsRegex}$");
        execute = function;
        AccessLevel = accessLevel;
    }

    public override string ToString()
    {
        return Name;
    }

    public async Task<bool> TryMatch(IMessage message, GuildSettings guildSettings)
    {
        if (!commandRegex.IsMatch(message.Content))
            return false;

        // TODO InvalidUse
        if (!commandRegexArgs.IsMatch(message.Content))
        {
            await InvalidUse();
            return true;
        }

        Match match = commandRegexArgs.Match(message.Content);

        AccessLevel accessLevel = guildSettings.AccessLevels.ContainsKey(message.Author.Id) ? guildSettings.AccessLevels[message.Author.Id] : 0;
        if (accessLevel < this.AccessLevel)
            return true;

        await execute(message, match.Groups);

        return true;
    }

    public async Task InvalidUse()
    {

    }
}

internal partial class BigBrother
{
    private async Task CommandReceived(SocketMessage message)
    {
        foreach (Command command in commands)
        {
            SocketGuild? guild = (message.Channel as SocketGuildChannel)?.Guild;
            if (guild == null || !guildSettings.ContainsKey(guild.Id))
                throw new Exception("Guild was not found");
            if (await command.TryMatch(message, guildSettings[guild.Id]))
                return;
        }
    }
}