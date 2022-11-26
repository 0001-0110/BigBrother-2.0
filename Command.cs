using Discord;
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
    private readonly static char prefix = '!';
    private readonly static string argPrefix = "--";

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
        string prefixedCommand = $"^{prefix}{name}";
        commandRegex = new Regex(prefixedCommand);
        if (argsRegex.Length == 0)
            commandRegexArgs = new Regex($"{prefixedCommand} {argsRegex}$");
        else
            commandRegexArgs = new Regex($"{prefixedCommand}$");
        execute = function;
        AccessLevel = accessLevel;
    }

    public async Task<bool> TryMatch(IMessage message, GuildSettings guildSettings)
    {
        if (!commandRegex.IsMatch(message.Content))
            return false;

        Match match = commandRegex.Match(message.Content);

        AccessLevel accessLevel = guildSettings.AccessLevels.ContainsKey(message.Author.Id) ? guildSettings.AccessLevels[message.Author.Id] : 0;
        if (accessLevel < this.AccessLevel)
            return false;

        await execute(message, match.Groups);

        return true;
    }
}
