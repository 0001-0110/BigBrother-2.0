using Discord;
using Discord.WebSocket;
using System.Text.RegularExpressions;

internal class Hangman
{
    private readonly static char emptyChar = '_';

    private static Random random = new Random();

    private string[] wordList;
    public bool IsPlaying;
    public char[] WordToGuess;
    public char[] CurrentGuess;
    public List<char> guessedLetters;

    public Hangman(string[] wordList)
    {
        this.wordList = wordList;
        IsPlaying = false;
        WordToGuess = new char[] { };
        CurrentGuess = new char[] { };
        guessedLetters = new List<char>();
    }

    private char[] GetRandomWord()
    {
        return wordList[random.Next(0, wordList.Length)].ToCharArray();
    }

    public string StartGame()
    {
        if (IsPlaying)
            return "The game is already in progress, use !guess to play";

        IsPlaying = true;
        WordToGuess = GetRandomWord();
        // Current guess is set to a word the same size with no letters
        CurrentGuess = new char[WordToGuess.Length];
        for (int i = 0; i < CurrentGuess.Length; i++)
            CurrentGuess[i] = emptyChar;
        guessedLetters.Clear();

        // TODO
        return "";
    }

    public string GuessLetter(char guessedLetter)
    {
        if (!IsPlaying)
            return "Start a new game before trying to guess";

        if (guessedLetters.Contains(guessedLetter))
            return "You already tried this letter";
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

        // TODO
        return "";
    }

    public string StopGame()
    {
        if (!IsPlaying)
            return "There is no game in progress";

        IsPlaying = false;
        return "Stop having fun!";
    }
}

internal partial class BigBrother
{
    private const string WORDLISTFOLDER = "Hangman";
    private const string WORDLISTFILE = "Words.txt";
    private string[]? wordList;

    private Dictionary<ulong, Hangman> hangmans = new Dictionary<ulong, Hangman>() { };

    private void InitHangman()
    {
        commands.Add(new Command("hangman", "` -> starts a new game of hangman (WIP)", StartHangman));
        commands.Add(new Command("guess", "([a-zA-Z])", " <letter>` -> guess a new letter for hangman (WIP)", GuessLetter));
        // TODO add stop
        //commands.Add(new Command("stopHangman, "` -> TODO", TODO, AccessLevel.Moderator));
    }

    private async Task LoadWords()
    {
        using (StreamReader file = new StreamReader(GetPath(WORDLISTFOLDER, WORDLISTFILE)))
            wordList = (await file.ReadToEndAsync()).Split(Environment.NewLine);
    }

    private async Task<Hangman> GetHangman(IMessage message)
    {
        SocketGuild? guild = GetGuild(message.Channel);
        if (guild == null)
            throw new Exception("Guild was not found");

        //
        if (wordList == null)
            await LoadWords();

        // If no hangman exists, create a new one
        if (!battles.ContainsKey(guild.Id))
            hangmans[guild.Id] = new Hangman(wordList!);

        return hangmans[guild.Id];
    }

    private async Task StartHangman(IMessage message, GroupCollection args)
    {
        Hangman hangman = await GetHangman(message);
        await SendMessage(message.Channel, hangman.StartGame());
    }

    private async Task GuessLetter(IMessage message, GroupCollection args)
    {
        Hangman hangman = await GetHangman(message);
        // Only take the first char since the arg have to be exactly one char long
        await SendMessage(message.Channel, hangman.GuessLetter(args[1].Value[0]));
    }

    private async Task StopHangman(IMessage message, GroupCollection args)
    {
        Hangman hangman = await GetHangman(message);
        await SendMessage(message.Channel, hangman.StopGame());
    }
}
