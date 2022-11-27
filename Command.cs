using Discord;
using Discord.WebSocket;
using System.Text.RegularExpressions;
using System.Threading.Channels;

enum AccessLevel
{
    Blacklist = -2,
    Limited = -1,
    Standard = 0,
    Moderator = 1,
    Admin = 2,
}

internal class Command
{
    public readonly static char Prefix = '!';

    public string Name;
    public string Help;
    private Regex commandRegex;
    private Regex commandRegexArgs;
    private Func<IMessage, GroupCollection, Task> execute;
    public AccessLevel AccessLevel;

    public Command(string name, string help, Func<IMessage, GroupCollection, Task> function, 
        AccessLevel accessLevel = AccessLevel.Standard)
        : this(name, "", help, function, accessLevel) { }

    public Command(string name, string argsRegex, string help, Func<IMessage, GroupCollection, Task> function, 
        AccessLevel accessLevel = AccessLevel.Standard)
    {
        Name = name;
        string prefixedCommand = $"^{Prefix}{name}";
        commandRegex = new Regex(prefixedCommand);
        if (argsRegex.Length == 0)
            commandRegexArgs = new Regex($"{prefixedCommand}$");
        else
            // " ?" between the command and the args to account for the space only if there is given args
            commandRegexArgs = new Regex($"{prefixedCommand} ?{argsRegex}$");
        Help = $"> `{Prefix}{name}{help}";
        execute = function;
        AccessLevel = accessLevel;
    }

    public override string ToString()
    {
        return Name;
    }

    public bool IsMatch(string message)
    {
        return commandRegex.IsMatch(message);
    }

    public async Task<bool> TryMatch(IMessage message)
    {
        if (!commandRegexArgs.IsMatch(message.Content))
            return false;

        Match match = commandRegexArgs.Match(message.Content);
        await execute(message, match.Groups);

        return true;
    }
}

internal partial class BigBrother
{
    private List<Command> commands;

    private void InitAllCommands()
    {
        commands.Add(new Command("help", "([a-zA-Z]*)", " <command=\"\">` -> Display help for the given command, or all available commands if none is given", Help, AccessLevel.Limited));
        InitQuit();
        InitDice();
        InitQuote();
        InitBattle();
        InitHangman();
    }

    private Command? GetCommand(string message)
    {
        foreach (Command command in commands)
            if (command.IsMatch(message))
                return command;
        return null;
    }

    private Command? GetCommandByName(string message)
    {
        foreach (Command command in commands)
            if (command.Name == message)
                return command;
        return null;
    }

    private GuildSettings GetGuildSettings(IChannel channel)
    {
        SocketGuild? guild = GetGuild(channel);
        if (guild == null)
            throw new Exception("Couldn't find the guild");
        return guildSettings[guild.Id];
    }

    private AccessLevel GetAccessLevel(GuildSettings guildSettings, IUser user)
    {
        if (guildSettings.AccessLevels.ContainsKey(user.Id))
            return guildSettings.AccessLevels[user.Id];
        return AccessLevel.Standard;
    }

    private async Task Help(IMessage message, string args = "")
    {
        string help = "Help:";
        if (args != "")
        {
            Command? command = GetCommandByName(args);
            if (command == null)
            {
                await Help(message);
                return;
            }
            else
                help += $"\n> {command.Help}";
        }
        else
        {
            GuildSettings activeGuildSettings = GetGuildSettings(message.Channel);
            AccessLevel userAccessLevel = GetAccessLevel(activeGuildSettings, message.Author);
            foreach (Command command in commands)
                if (userAccessLevel >= command.AccessLevel)
                    help += $"\n> {command.Help}";
        }
        await SendMessage(message.Channel, help);
    }

    private async Task Help(IMessage message, GroupCollection args)
    {
        if (args[1].Value != "")
            await Help(message, args[1].Value);
        else
            await Help(message);
    }

    private async Task InvalidUse(IMessage message, string commandName)
    {
        await SendMessage(message.Channel, "Invalid use of this command, this might help you:");
        await Help(message, commandName);
    }

    private async Task UnauthorizedUse(IMessage message)
    {
        await SendMessage(message.Channel, "You do not have the permission to use this command");
    }

    private async Task CommandReceived(IMessage message)
    {
        Command? command = GetCommand(message.Content);
        if (command == null)
        {
            await SendMessage(message.Channel, "Unknown command");
            await Help(message);
            return;
        }

        GuildSettings activeGuildSettings = GetGuildSettings(message.Channel);
        AccessLevel userAccessLevel = GetAccessLevel(activeGuildSettings, message.Author);
        if (userAccessLevel < command.AccessLevel)
        {
            await UnauthorizedUse(message);
            return;
        }

        if (!await command.TryMatch(message))
        {
            await InvalidUse(message, command.Name);
        }
    }
}
