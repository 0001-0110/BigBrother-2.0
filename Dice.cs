using Discord;
using System.Text.RegularExpressions;

internal partial class BigBrother
{
    private void InitDice()
    {
        commands.Add(new Command("dice", "([0-9]*) ?([0-9]*)", " <max=6> <throws=1>` -> Roll `throws` time a dice with `max` side", Dice));
    }

    private async Task DiceRoll(IMessageChannel channel, int max)
    {
        await SendMessage(channel, $"You rolled a {random.Next(1, max)}");
    }

    private async Task Dice(IMessage message, GroupCollection args)
    {

        if (args[2].Value != "")
        {
            int rolls = int.Parse(args[2].Value);
            for (int i = 0; i < rolls; i++)
                await DiceRoll(message.Channel, int.Parse(args[1].Value));
            return;
        }

        if (args[1].Value != "")
        {
            await DiceRoll(message.Channel, int.Parse(args[1].Value));
            return;
        }

        await DiceRoll(message.Channel, 6);
    }
}
