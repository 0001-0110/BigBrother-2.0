using Discord;
using System.Net;
using System.Text.RegularExpressions;

internal class Writer
{
    internal class WritingTree
    {
        private string word;
        private List<WritingTree> children;
        private List<(float, string)> nextWords;

        public WritingTree(string word="")
        {
            this.word = word;
        }

        public void AddText(string text, int depth)
        {
            
        }

        private void AddWord()
        {

        }

        public string GetNextWord(IEnumerable<string> previousWords, int index=0)
        {
            // TODO

            if (index < previousWords.Count())
                // Search if any of the children corresponds to the previous words
                foreach (WritingTree child in children)
                    if (child.word == previousWords.ElementAt(index))
                        return child.GetNextWord(previousWords, index + 1);

            // 
            return "";
        }
    }

    private int depth;
    private WritingTree writingTree;

    public Writer(List<string> texts) : this(texts.ToArray()) { }

    public Writer(string[] texts, int depth=7)
    {
        this.depth = depth;
        writingTree = new WritingTree();
        foreach (string text in texts)
            writingTree.AddText(text, depth);
    }

    public string Write(int limit=6000)
    {
        string text = "";

        Queue<string> previousWords = new Queue<string>(depth);
        previousWords.Enqueue("");

        while (true)
        {
            writingTree.GetNextWord(previousWords);
        }

        return text;
    }
}

internal partial class BigBrother
{
    private static string TEXTFOLDER = "Texts";

    Dictionary<string, Writer> writers = new Dictionary<string, Writer>();

    private void InitWriter()
    {
        commands.Add(new Command("createWriter", "", "", CreateWriter));
        commands.Add(new Command("writerList", "", WriterList));
        commands.Add(new Command("write", "", "", Write));

        //
        client.Ready += InitWriters;
    }

    private async Task InitWriters()
    {
        // Init all built in writers
        foreach (string folder in Directory.GetDirectories(GetPath(TEXTFOLDER)))
        {
            List<string> texts = new List<string>();
            string[] files = Directory.GetFiles(GetPath(folder));
            foreach (string file in files)
                using (StreamReader streamReader = new StreamReader(file))
                    texts.Add(await streamReader.ReadToEndAsync());
        }
    }

    private async Task CreateWriter(IMessage message, GroupCollection args)
    {
        // TODO

        // Check if the name is already taken
        if (writers.ContainsKey(args[1].Value))
        {
            await SendMessage(message.Channel, "This name is already taken, please choose antother one");
            return;
        }

        HttpClient client = new HttpClient();
        List<string> texts = new List<string>();

        foreach (Attachment file in message.Attachments)
        {
            // TODO

            //texts.Add();
        }

        writers[args[1].Value] = new Writer(texts);
    }

    private async Task WriterList(IMessage message, GroupCollection args)
    {
        string writerList = string.Join("\n", "Here is the list of all writers you can use:", writers.Keys.ToArray());
        await SendMessage(message.Channel, writerList);
    }

    private async Task Write(IMessage message, GroupCollection args)
    {
        if (!writers.ContainsKey(args[1].Value))
        {
            await SendMessage(message.Channel, "This writer does not exist");
            return;
        }

        await SendMessage(message.Channel, writers[args[1].Value].Write());
    }
}
