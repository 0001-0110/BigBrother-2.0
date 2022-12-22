using Discord;
using Discord.WebSocket;
using System.Text.RegularExpressions;

internal partial class BigBrother
{
    void InitRoles()
    {
        commands.Add(new Command("getrole", " <@&([0-9]+)>", " <role>` -> Gives you the role", GetRole));
        //commands.Add(new Command("", "", "", ));
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
        ulong roleId;
        if (!ulong.TryParse(args[1].Value, out roleId))
        {
            // Id is invalid
            await SendMessage(message.Channel, "");
            return;
        }
        IRole? role = guild.Roles.FirstOrDefault(role => role.Id == roleId);
        if (role == null)
        {
            // Role does not exist
            await SendMessage(message.Channel, "");
            return;
        }
        if (!guildSettings[guild.Id].OnDemandRoles.Contains(roleId))
        {
            // Role cannot be given
        }

        await (message.Author as IGuildUser).AddRoleAsync(role);
        await SendMessage(message.Channel, $"{MentionUtils.MentionUser(message.Author.Id)}: {role.Name} role added");
    }
}
