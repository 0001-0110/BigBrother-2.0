using Discord;
using System.Text.RegularExpressions;

internal partial class BigBrother
{
    private void InitRemindMe()
    {
        commands.Add(new Command("remindMe", ".*", " <duration> <text>` -> Wait `duration` before sending you back `text` (WIP)", RemindMe));
    }

    private async Task RemindMe(IMessage message, GroupCollection args)
    {
        throw new NotImplementedException();
    }
}
