using System.Text.RegularExpressions;

internal class Command
{
    Regex commandRegex;
    Func<string[]> execute;
    bool adminOnly;
   
    public Command(Regex commandRegex, Func<string[]> function, bool adminOnly=false, )
    {
        this.commandRegex = commandRegex;
        execute = function;
        this.adminOnly = adminOnly;
    }

    public bool TryMatch(string message)
    {
        if (!commandRegex.IsMatch(message))
            return false;

        Match match = commandRegex.Match(message);
        execute(match.Groups.Values);
        return true;
    }
}
