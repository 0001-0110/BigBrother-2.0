using Discord;
using System.Text.RegularExpressions;

internal partial class BigBrother
{
    private void InitGit()
    {
        commands.Add(new Command("git", "` -> The link to the git repo of Big Brother", SendGitRepo));
    }

    private async Task SendGitRepo(IMessage message, GroupCollection args)
    {
        await SendMessage(message.Channel, "https://github.com/0001-0110/BigBrother");
    }
}
