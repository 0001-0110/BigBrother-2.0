﻿using Discord;
using Discord.WebSocket;
using System.Text.RegularExpressions;

internal class Battle
{
    internal class Item
    {
        public string Name;

        public Item(string name)
        {
            Name = name;
        }
    }

    internal class Event
    {
        private Item requiredItem;
        private string winEvent;
        private string loseEvent;
        public uint Kills;

        public Event(Item requiredItem, string winEvent, string loseEvent)
        {
            this.requiredItem = requiredItem;
            this.winEvent = winEvent;
            this.loseEvent = loseEvent;
            Kills = 0;
        }

        public Event(string itemName, string winEvent, string loseEvent)
            : this(new Item(itemName), winEvent, loseEvent) { }

        public IEnumerable<string> Happen(Player player, List<Player> alivePlayers)
        {
            if (player.Items.Contains(requiredItem))
            {
                player.Items.Remove(requiredItem);
                yield return $"{{{player.Id}}} {winEvent}";
            }
            else
            {
                player.IsAlive = false;
                alivePlayers.Remove(player);
                Kills++;
                yield return $"{{{player.Id}}} {loseEvent}";
            }
        }

        public IEnumerable<string> FindItem(Player player)
        {
            player.Items.Add(requiredItem);
            yield return $"{{{player.Id}}} found {requiredItem.Name}";
        }
    }

    internal class Player
    {
        public ulong Id;
        public string Name;
        public bool IsAlive;
        public List<Item> Items;

        public Player(ulong id, string name)
        {
            Id = id;
            Name = name;
            IsAlive = true;
            Items = new List<Item>();
        }
    }

    private Random random = new Random();

    private List<Event> events;

    public bool IsActive;
    public bool IsPlaying;
    private List<Player> players;
    private List<Player> alivePlayers;

    public Battle(string filename)
    {
        IsActive = false;
        IsPlaying = false;
        players = new List<Player>();
        alivePlayers = new List<Player>();

        LoadEvents(filename);
    }

    private void LoadEvents(string filename)
    {
        events = new List<Event>();
        using (StreamReader streamReader = new StreamReader(filename))
        {
            foreach (string line in streamReader.ReadToEnd().Split("\n"))
            {
                string[] args = line.Split("");
                if (args.Length == 3)
                    events.Add(new Event(args[0], args[1], args[2]));
            }
        }
    }

    public IEnumerable<string> StartGame()
    {
        // TODO
        if (IsActive)
        {
            yield return "";   
        }
        else
        {
            IsActive = true;
            players.Clear();
            alivePlayers.Clear();
            yield return "";
        }
    }

    public IEnumerable<string> JoinGame(IUser player)
    {
        // TODO
        if (!IsActive)
        {
            yield return "";
        }
        else if (IsPlaying)
        {
            yield return "";
        }
        else
        {
            Player newPlayer = new Player(player.Id, player.Username);
            players.Add(newPlayer);
            alivePlayers.Add(newPlayer);
            yield return "";
        }
    }

    public IEnumerable<string> PlayGame()
    {
        IsPlaying = true;
        while (IsPlaying && alivePlayers.Count > 1)
        {
            Player activePlayer = alivePlayers[random.Next(0, alivePlayers.Count)];
            if (random.Next(0, 2) == 0)
            {
                foreach (string message in events[random.Next(0, events.Count)].Happen(activePlayer, alivePlayers))
                    yield return message;
            }
            else
            {
                foreach (string message in events[random.Next(0, events.Count)].FindItem(activePlayer))
                    yield return message;
            }
        }

        yield return $"{alivePlayers[0].Name} is the winner !";
    }

    public void StopGame()
    {
        IsActive = false;
        IsPlaying = false;
    }
}

internal partial class BigBrother
{
    private const string EVENTFOLDER = "C:\\Users\\remi\\OneDrive\\Documents\\Travail\\Prog\\C#\\BigBrother\\BigBrother\\Data";
    private Dictionary<ulong, Battle> battles = new Dictionary<ulong, Battle>() { };

    private void InitBattle()
    {
        commands.Add(new Command("battle", "` -> start a new battle for players to join", StartBattle));
        commands.Add(new Command("joinBattle", "` -> Join the current battle", JoinBattle));
        commands.Add(new Command("playBattle", "` -> Close the current battle to new players and starts the game", PlayBattle));
        commands.Add(new Command("stopBattle", "` -> Stop the current battle", StopBattle, AccessLevel.Moderator));
    }

    private Battle? GetBattle(IMessage message)
    {
        SocketGuild? guild = GetGuild(message.Channel);
        if (guild == null)
            throw new Exception("Guild was not found");
        if (!battles.ContainsKey(guild.Id))
            if (guildSettings[guild.Id].EventsFile != null)
                battles[guild.Id] = new Battle($"{EVENTFOLDER}\\{guildSettings[guild.Id].EventsFile}");
        return battles[guild.Id];
    }

    private async Task StartBattle(IMessage message, GroupCollection args)
    {
        Battle? battle = GetBattle(message);
        if (battle == null)
        {
            await SendMessage(message.Channel, "Sadly, battles are not available on this server");
            return;
        }

        foreach (string thing in battle.StartGame())
            await SendMessage(message.Channel, thing);
    }

    private async Task JoinBattle(IMessage message, GroupCollection args)
    {
        Battle? battle = GetBattle(message);
        if (battle == null)
        {
            await SendMessage(message.Channel, "Couldn't load the battle");
            return;
        }

        foreach (string thing in battle.JoinGame(message.Author))
            await SendMessage(message.Channel, thing);
    }

    private async Task PlayBattle(IMessage message, GroupCollection args)
    {
        Battle? battle = GetBattle(message);
        if (battle == null)
        {
            await SendMessage(message.Channel, "");
            return;
        }

        foreach (string thing in battle.PlayGame())
            await SendMessage(message.Channel, thing);
    }

    private async Task StopBattle(IMessage message, GroupCollection args)
    {
        Battle? battle = GetBattle(message);
        if (battle == null)
        {
            await SendMessage(message.Channel, "");
            return;
        }

        battle.StopGame();
    }
}