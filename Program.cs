public static class Program
{
    private static string tokenFile = "C:\\Users\\remi\\OneDrive\\Documents\\travail\\Prog\\C#\\BigBrother\\BigBrother\\Data\\token.txt";

    static async Task Main(string[] args)
    {
        await new BigBrother().Run(tokenFile);
    }
}
