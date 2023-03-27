using Discord;
using Discord.WebSocket;
using System.Text.RegularExpressions;
using Services;

internal partial class Extensions
{
    public static bool AlmostEqual(this string str1, string str2, float threshold, bool caseSensitive = true)
    {
        // TODO
        return (StringService.LevenshteinDistance(str1, str2, caseSensitive)) <= IntService.Max(str1.Length, str2.Length) * threshold;
    }
}

internal partial class BigBrother
{
    void InitRoles()
    {
        commands.Add(new Command("getrole", " (.+)", " <role>` -> Gives you the role", AddRole));
        commands.Add(new Command("removerole", " (.+)", " <role>` -> Removes the role from you", RemoveRole));
        commands.Add(new Command("rolelist", "` -> Display the list of all roles that you can demand on this guild", RoleList));
    }

    private SocketRole? GetRole(ulong roleId, SocketGuild guild)
    {
        foreach (SocketRole role in guild.Roles)
            if (role.Id == roleId)
                return role;
        return null;
    }

    private SocketRole? GetRole(string roleName, SocketGuild guild)
    {
        foreach (SocketRole role in guild.Roles)
            if (role.Name.AlmostEqual(roleName, 0.25f, false))
                return role;
        return null;
    }

    private async Task AddRole(IMessage message, GroupCollection args)
    {
        if (message.Channel.GetChannelType() == ChannelType.DM)
        {
            // Roles are not available in private messages
            await SendMessage(message.Channel, "What exactly were you hoping for ?");
            return;
        }

        SocketGuild? guild = GetGuild(message.Channel);

        SocketRole? newRole = GetRole(args[1].Value, guild);
        if (newRole == null)
        {
            await SendMessage(message.Channel, "This role does not exist, maybe you should check the spelling");
            return;
        }

        if (!guildSettings[guild.Id].OnDemandRoles.Any(roleId => roleId == newRole.Id))
        {
            await SendMessage(message.Channel, "This role is not on demand");
            return;
        }

        // TODO handle warning
        if ((message.Author as SocketGuildUser).Roles.Any(role => role.Id == newRole.Id))
        {
            await SendMessage(message.Channel, "You already have this role, you can remove it with `!removerole <role>`");
            return;
        }

        await (message.Author as IGuildUser).AddRoleAsync(newRole);
        await SendMessage(message.Channel, $"{MentionUtils.MentionUser(message.Author.Id)}: {newRole.Name} role added");
    }

    private async Task RemoveRole(IMessage message, GroupCollection args)
    {
        if (message.Channel.GetChannelType() == ChannelType.DM)
        {
            // Roles are not available in private messages
            await SendMessage(message.Channel, "???");
            return;
        }

        SocketGuild? guild = GetGuild(message.Channel);

        SocketRole? newRole = GetRole(args[1].Value, guild);
        if (newRole == null)
        {
            await SendMessage(message.Channel, "This role does not exist, maybe you should check the spelling");
            return;
        }

        if (!guildSettings[guild.Id].OnDemandRoles.Any(roleId => roleId == newRole.Id))
        {
            await SendMessage(message.Channel, "This role is not on demand");
            return;
        }

        // TODO handle warning
        if (!(message.Author as SocketGuildUser).Roles.Any(role => role.Id == newRole.Id))
        {
            await SendMessage(message.Channel, "You don't have this role, you can get it with `!getrole <role>`");
            return;
        }

        await (message.Author as IGuildUser).RemoveRoleAsync(newRole);
        await SendMessage(message.Channel, $"{MentionUtils.MentionUser(message.Author.Id)}: {newRole.Name} role removed");
    }

    private async Task RoleList(IMessage message, GroupCollection args)
    {
        SocketGuild? guild = GetGuild(message.Channel);
        
        if (guildSettings[guild.Id].OnDemandRoles.Length == 0)
        {
            await SendMessage(message.Channel, "There is no on demand role in this server");
            return;
        }

        string answer = "> You can demand any of these roles:\n";
        foreach (ulong roleId in guildSettings[guild.Id].OnDemandRoles)
        {
            answer += $"> `{GetRole(roleId, guild).Name}`\n";
        }
        await SendMessage(message.Channel, answer);
    }
}
