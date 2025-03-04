using Discord;
using Discord.WebSocket;
using System.Text.RegularExpressions;

internal partial class BigBrother
{
    private IEnumerable<string> _tables = new string[]
    {
        "(╯°□°)╯︵ ┻━┻",
        "ヽ(ຈل͜ຈ)ﾉ︵ ┻━┻",
        "┳━┳ ヽ(ಠل͜ಠ)ﾉ",
        "┬─┬ノ( º _ ºノ)",
        "(╯°Д°)╯︵/(.□ . \\)",
        "(╯°□°)╯︵ ʞooqǝɔɐℲ",
        "(ノಠ益ಠ)ノ彡┻━┻",
        "(┛◉Д◉)┛彡┻━┻",
        "(┛ಠ_ಠ)┛彡┻━┻",
        "┻━┻︵ \\(°□°)/ ︵ ┻━┻",
        "(˚Õ˚)ر ~~~~╚╩╩╝",
        "┏━┓┏━┓┏━┓ ︵ /(^.^/)",
        "(☞ﾟヮﾟ)☞ ┻━┻",
        "┻━┻ ︵╰(°□°╰)",
    };

    private void InitTable()
    {
        commands.Add(new Command("table", " -> table", Table));
    }

    private async Task Table(IMessage message, GroupCollection args)
    {
        await SendMessage(message.Channel, _tables.ElementAt(new Random().Next(_tables.Count())));
    }
}
