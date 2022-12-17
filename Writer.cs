using Discord;
using System.Text.RegularExpressions;

internal static partial class Extensions
{
    public static ulong NextUInt64(this Random random, ulong max)
    {
        return random.NextUInt64(0, max);
    }

    public static ulong NextUInt64(this Random random, ulong min, ulong max)
    {
        return (ulong)(random.NextInt64((long)(min - long.MaxValue), (long)(max - long.MaxValue))) + long.MaxValue;
    }
}

internal class Writer
{
    internal class WritingTree
    {
        private static Random random = new Random();

        private string word;
        private List<WritingTree> children;
        // Store the number of time this particular word combination appeared
        private ulong occurences;
        // Store all of the words that can follow, associated with the number of times it appeared
        private Dictionary<string, ulong> nextWords;

        public WritingTree(string word = "")
        {
            // Only the root of the tree has the empty string as a word
            this.word = word;
            children = new List<WritingTree>();
            occurences = 0;
            nextWords = new Dictionary<string, ulong>();
        }

        // TODO comment this or you'll regret this later
        public async Task AddWord(string word, IEnumerable<string> previousWords, int depth = 0)
        {
            // TODO need some serious testing

            int length = previousWords.Count() - 1;
            if (depth > length)
            {
                // We reached the end of the word combination
                // 
                occurences++;
                if (!nextWords.ContainsKey(word))
                    nextWords[word] = 1;
                else
                    nextWords[word]++;
            }
            else
            {
                // We search if the next word is already is the list of children
                foreach (WritingTree child in children)
                    if (child.word == previousWords.ElementAt(length - depth))
                    {
                        await child.AddWord(word, previousWords, depth + 1);
                        return;
                    }

                // There is no child existing for the next word of this combination
                WritingTree newWritingTree = new WritingTree(previousWords.ElementAt(length - depth));
                children.Add(newWritingTree);
                await newWritingTree.AddWord(word, previousWords, depth + 1);
            }
        }

        public async Task<string> GetNextWord(IEnumerable<string> previousWords, int index = 0)
        {
            // TODO

            // I'm not sure what this does
            await Task.Yield();

            if (index < previousWords.Count())
                // Search if any of the children corresponds to the previous words
                foreach (WritingTree child in children)
                    if (child.word == previousWords.ElementAt(index))
                        return await child.GetNextWord(previousWords, index + 1);

            // There is no more child with the correct word combination
            // We return a random-ish word from this node, according to the weights
            // First we pick a random value between 0 and max
            // We then sreach for the first word where the sum of all occurences is higher than this value
            ulong randomValue = random.NextUInt64(occurences);
            ulong sum = 0;
            foreach (string nextWord in nextWords.Keys)
            {
                sum += nextWords[nextWord];
                // If the sum is higher than the random value, this is the correct word to pick
                if (sum >= randomValue)
                    return nextWord;
            }

            // Let's hope we never reach this part
            throw new Exception("Achievement unlocked: How did we get here ?");
        }
    }

    private const int DEFAULTDEPTH = 7;

    private WritingTree writingTree;

    public Writer()
    {
        writingTree = new WritingTree();
    }

    public async Task AddTexts(string[] texts, int depth=DEFAULTDEPTH)
    {
        foreach (string text in texts)
        {
            Queue<string> previousWords = new Queue<string>(depth);
            // The empty string denotes the start of the text
            //previousWords.Enqueue("");
            // TODO split on all separators ?
            foreach (string word in text.Split(" "))
            {
                await writingTree.AddWord(word, previousWords);
                // 
                if (previousWords.Count >= depth)
                    previousWords.Dequeue();
                //
                previousWords.Enqueue(word);
            }
        }
    }

    public async Task AddTexts(List<string> texts, int depth=DEFAULTDEPTH)
    {
        await AddTexts(texts.ToArray(), depth);
    }

    public async Task<string> Write(int depth=DEFAULTDEPTH, int length=6000)
    {
        string text = "";

        Queue<string> previousWords = new Queue<string>();
        previousWords.Enqueue("");

        // Generate new words to fill the text
        while (text.Length < length)
        {
            string nextWord = await writingTree.GetNextWord(previousWords);
            previousWords.Enqueue(nextWord);
            if (previousWords.Count() > depth)
                previousWords.Dequeue();
            text += nextWord;
        }

        return text;
    }
}

internal partial class BigBrother
{
    // The folder containing all the subfolder
    // Each subfolder must contains text files
    // that are gonna be used to create the different writers
    private static string TEXTFOLDER = "Texts";

    // writers is initialize only when a user uses one of the commands
    // because creating a writer is really expensive
    Dictionary<string, Writer>? writers;

    private void InitWriter()
    {
        commands.Add(new Command("createwriter", "(.+)", " <name>` -> Creates a new writer with the given name (WIP)", CreateWriter));
        commands.Add(new Command("writerlist", "` -> Display the different writers available at that time", WriterList));
        commands.Add(new Command("write", " (.+)", " <name>` -> Crashes everything because someone coded with their feet (WIP)", Write));
    }

    // Init all the writers included in the app
    private async Task InitBuiltInWriters()
    {
        writers = new Dictionary<string, Writer>();

        // No folder means nothing to initialize
        if (!Directory.Exists(GetPath(TEXTFOLDER)))
            return;

        // Init all built in writers
        foreach (string folder in Directory.GetDirectories(GetPath(TEXTFOLDER)))
        {
            if (!folder.Contains("Grille"))
                continue;

            List<string> texts = new List<string>();
            foreach (string file in Directory.EnumerateFiles(folder))
                using (StreamReader streamReader = new StreamReader(file))
                    texts.Add(await streamReader.ReadToEndAsync());
            
            Writer newWriter = new Writer();
            await newWriter.AddTexts(texts);
            // TODO might need to look into this warning
            writers.Add(new DirectoryInfo(folder).Name, newWriter);
        }
    }

    private async Task CreateWriter(IMessage message, GroupCollection args)
    {
        // TODO

        if (writers == null)
            await InitBuiltInWriters();

        // Check if the name is already taken
        if (writers!.ContainsKey(args[1].Value))
        {
            await SendMessage(message.Channel, "This name is already taken, please choose another one");
            return;
        }

        HttpClient httpClient = new HttpClient();
        List<string> texts = new List<string>();

        foreach (Attachment file in message.Attachments)
        {
            // TODO
            // Get the Url of the file attached to the message
            string url = file.Url;
            // Download the files
            var response = await httpClient.GetAsync(url);
            // Save the text result
            texts.Add(await response.Content.ReadAsStringAsync());
        }

        Writer newWriter = new Writer();
        await newWriter.AddTexts(texts);
        writers[args[1].Value] = newWriter;

        await SendMessage(message.Channel, $"Successfully created new writer {args[1].Value}");
    }

    private async Task WriterList(IMessage message, GroupCollection args)
    {
        if (writers == null)
            await InitBuiltInWriters();

        string writerList = "Here is the list of all the writers you can use:\n";
        foreach (string writerName in writers!.Keys.ToArray())
            writerList += $"{writerName}\n";
        await SendMessage(message.Channel, writerList);
    }

    private async Task Write(IMessage message, GroupCollection args)
    {
        if (writers == null)
            await InitBuiltInWriters();

        if (!writers!.ContainsKey(args[1].Value))
        {
            await SendMessage(message.Channel, "This writer does not exist");
            return;
        }

        // Generates a random text
        string text = await writers[args[1].Value].Write();
        // Save it in a file
        string filename = GetPath(TEXTFOLDER, $"Text_{args[1].Value}.txt");
        using (StreamWriter streamWriter = new StreamWriter(filename))
            await streamWriter.WriteLineAsync(text);
        // Send the file
        await message.Channel.SendFileAsync(filename, "TODO");
        // Delete the local save of the file
        File.Delete(filename);
    }
}
