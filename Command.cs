using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Diagnostics;
using System.Text.RegularExpressions;

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

    public Command(string name, string help, 
        Func<IMessage, GroupCollection, Task> function, AccessLevel accessLevel = AccessLevel.Standard)
        : this(name, "", help, function, accessLevel) { }

    public Command(string name, string argsRegex, string help,
        Func<IMessage, GroupCollection, Task> function, AccessLevel accessLevel = AccessLevel.Standard)
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
        commands.Add(new Command("help", "([a-zA-Z]*)", "` -> You do need some", Help, AccessLevel.Limited));
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
            if (command.IsMatch(message))
                return command;
        return null;
    }

    private async Task Help(IMessage message, string args = "")
    {
        string help = "Help:";
        if (args != "")
        {
            // TODO
            Command? command = GetCommandByName(args);
            if (command == null)
                //await Help();
                throw new NotImplementedException();
            else
                help += $"\n> {command.Help}";
        }
        else
        {
            foreach (Command command in commands)
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

    private async Task InvalidUse(IMessage message)
    {
        throw new NotImplementedException();
    }

    private async Task UnauthorizedUse(IMessage message)
    {
        throw new NotImplementedException();
    }

    private async Task CommandReceived(IMessage message)
    {
        Command? command = GetCommand(message.Content);
        if (command == null)
        {
            // Unknown command
            // TODO Display help
            await Help(message);
            return;
        }

        // TODO handle that safely
        SocketGuild? guild = GetGuild(message.Channel);
        AccessLevel userAccessLevel = guildSettings[guild.Id].AccessLevels[message.Author.Id];
        if (userAccessLevel < command.AccessLevel)
        {
            await UnauthorizedUse(message);
            return;
        }

        if (!await command.TryMatch(message))
        {
            await InvalidUse(message);
        }
    }
}