﻿using Discord;
using System.Text.RegularExpressions;

internal class Hangman
{
    private readonly static string wordListFile = "";
    private static string[]? wordList;
    private readonly static char emptyChar = '_';

    private Random random;

    public bool IsActive;
    public string? WordToGuess;
    public string? CurrentGuess;

    private static void LoadWordList()
    {
        using (StreamReader file = new StreamReader(wordListFile))
            wordList = file.ReadToEnd().Split("\n");
    }

    public Hangman()
    {
        random = new Random();

        IsActive = false;

        if (wordList == null)
            LoadWordList();
    }


    public void StartGame()
    {
        IsActive = true;
        WordToGuess = wordList[random.Next(0, wordList.Length)];
        CurrentGuess = new string(emptyChar, WordToGuess.Length);
    }
}

internal partial class BigBrother
{
    private Command startHangman;
    private Command guessLetter;

    private void InitHangman()
    {
        startHangman = new Command("[Hh]angman", StartHangman);
        guessLetter = new Command("[Gg]uess", "([a-zA-Z])", GuessLetter);
    }

    private async Task StartHangman(IMessage message, GroupCollection args)
    {

    }

    private async Task GuessLetter(IMessage message, GroupCollection args)
    {

    }
}
