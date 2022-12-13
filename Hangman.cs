using Discord;
using Discord.WebSocket;
using System.Text.RegularExpressions;

internal class Hangman
{
    private readonly static string wordListFile = "";
    private static string[]? wordList;
    private readonly static char emptyChar = '_';

    private static Random random = new Random();

    public bool IsPlaying;
    public char[] WordToGuess;
    public char[] CurrentGuess;
    public List<char> guessedLetters;

    private static char[] GetRandomWord()
    {
        // TODO might be better to load them again rather than keeping them in an array

        if (wordList == null)
            using (StreamReader file = new StreamReader(wordListFile))
                wordList = file.ReadToEnd().Split("\n");

        return wordList[random.Next(0, wordList.Length)].ToCharArray();
    }

    public Hangman()
    {
        IsPlaying = false;
        WordToGuess = new char[] { };
        CurrentGuess = new char[] { };
        guessedLetters = new List<char>();
    }

    public string StartGame()
    {
        IsPlaying = true;
        WordToGuess = GetRandomWord();
        // TODO fill this array
        CurrentGuess = new char[] { };
        guessedLetters.Clear();

        // TODO
        return "";
    }

    public string GuessLetter(char guessedLetter)
    {
        if (guessedLetters.Contains(guessedLetter))
            // TODO
            return "";
        guessedLetters.Add(guessedLetter);

        bool correctGuess = false;
        for (int i = 0; i < WordToGuess.Length; i++)
            if (guessedLetter == WordToGuess[i])
            {
                correctGuess = true;
                CurrentGuess[i] = guessedLetter;
            }

        if (correctGuess)
        { }

        return "";
    }
}

internal partial class BigBrother
{
    private Dictionary<ulong, Hangman> hangmans = new Dictionary<ulong, Hangman>() { };

    private void InitHangman()
    {
        commands.Add(new Command("hangman", "` -> starts a new game of hangman (WIP)", StartHangman));
        commands.Add(new Command("guess", "([a-zA-Z])", " <letter>` -> guess a new letter for hangman (WIP)", GuessLetter));
        // TODO add stop
    }

    private Hangman GetHangman(IMessage message)
    {
        SocketGuild? guild = GetGuild(message.Channel);
        if (guild == null)
            throw new Exception("Guild was not found");

        // If no hangman exists, create a new one
        if (!battles.ContainsKey(guild.Id))
            hangmans[guild.Id] = new Hangman();

        return hangmans[guild.Id];
    }

    private async Task StartHangman(IMessage message, GroupCollection args)
    {
        Hangman hangman = GetHangman(message);

        if (hangman.IsPlaying)
        {
            await SendMessage(message.Channel, "The game is already in progress, use !guess to play");
            return;
        }

        await SendMessage(message.Channel, hangman.StartGame());
    }

    private async Task GuessLetter(IMessage message, GroupCollection args)
    {
        Hangman hangman = GetHangman(message);

        if (!hangman.IsPlaying)
        {
            await SendMessage(message.Channel, "Start a new game before trying to guess");
            return;
        }

        // Only take the first char since the args have to be exactly one char long
        await SendMessage(message.Channel, hangman.GuessLetter(args[1].Value[0]));
    }
}
