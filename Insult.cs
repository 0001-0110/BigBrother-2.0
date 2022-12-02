using Discord;
using System.Text.RegularExpressions;

internal partial class BigBrother
{
    private const string INSULTFOLDER = "Insult";
    private const string INSULTFILE = "Insults.txt";
    private static string[]? insults;

    private void InitInsult()
    {
        // TODO find a regex that accepts mentions
        commands.Add(new Command("insult", "", "` -> Do I really need to explain this one to you ? ", Insult));
    }

    private void LoadInsults()
    {
        using (StreamReader streamReader = new StreamReader(GetPath(INSULTFOLDER, INSULTFILE)))
            insults = streamReader.ReadToEnd().Split("\n");
    }

    private async Task Insult(IMessage message, GroupCollection args)
    {
        // TODO use the mention to target the insult
        if (insults == null)
            LoadInsults();
        await SendMessage(message.Channel, insults![random.Next(0, insults.Length)]);
    }
}
