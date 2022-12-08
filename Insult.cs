using Discord;
using System.Text.RegularExpressions;

internal partial class BigBrother
{
    private const string INSULTFOLDER = "Insult";
    private const string INSULTFILE = "Insults.txt";
    private static string[]? insults;

    private void InitInsult()
    {
        commands.Add(new Command("insult", "(?: <@([0-9]+)>)?", "` -> Do I really need to explain this one to you ? ", Insult));
    }

    private void LoadInsults()
    {
        using (StreamReader streamReader = new StreamReader(GetPath(INSULTFOLDER, INSULTFILE)))
            insults = streamReader.ReadToEnd().Split("\n");
    }

    private async Task Insult(IMessage message, GroupCollection args)
    {
        if (insults == null)
            LoadInsults();

        string insult = insults![random.Next(0, insults.Length)];

        if (args[1].Value != "")
        {
            ulong userId;
            if (!ulong.TryParse(args[1].Value, out userId))
            {
                await SendMessage(message.Channel, "Incorrect user ID");
                return;
            }

            // TODO is redundant with SendMessage
            string mention = MentionUtils.MentionUser(userId);
            if (mention == "")
            {
                await SendMessage(message.Channel, "Incorrect user ID");
                return;
            }

            await SendMessage(message.Channel, $"{mention} {insult}");
        }
        else
        {
            await SendMessage(message.Channel, insult);
        }
    }
}
