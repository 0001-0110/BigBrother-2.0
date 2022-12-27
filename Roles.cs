using Discord;
using Discord.WebSocket;
using System.Text.RegularExpressions;

internal partial class Extensions
{
    public static bool AlmostEqual(this string str1, string str2)
    {
        // TODO
        return (StringService.LevenshteinDistance(str1, str2)) <= 2;
    }
}

internal partial class BigBrother
{
    void InitRoles()
    {
        commands.Add(new Command("getrole", " (.+)", " <role>` -> Gives you the role", GetRole));
        commands.Add(new Command("removerole", " (.+)", " <role>` -> Removes the role from you", RemoveRole));
    }

    private async Task GetRole(IMessage message, GroupCollection args)
    {
        // TODO warnings ?

        if (message.Channel.GetChannelType() == ChannelType.DM)
        {
            // Roles are not available in private messages
            await SendMessage(message.Channel, "What exactly were you hoping for ?");
            return;
        }
        
        SocketGuild guild = GetGuild(message.Channel);
        foreach (ulong roleId in guildSettings[guild.Id].OnDemandRoles)
        {
            SocketRole role = guild.GetRole(roleId);
            if (role.Name.AlmostEqual(args[1].Value))
            {
                await (message.Author as IGuildUser).AddRoleAsync(role);
                await SendMessage(message.Channel, $"{MentionUtils.MentionUser(message.Author.Id)}: {role.Name} role added");
                return;
            }
        }

        await SendMessage(message.Channel, "There is no role with this name, or this role is not on demand");
    }

    private async Task RemoveRole(IMessage message, GroupCollection args)
    {
        await SendMessage(message.Channel, $"Not implemented yet, ask {MentionUtils.MentionUser(315827580869804042)} to do it");
    }
}
