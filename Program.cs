using System.Reflection;

public static class Program
{
    private static string dataFolder = "C:\\Users\\remi\\OneDrive\\Documents\\travail\\Prog\\C#\\BigBrother\\BigBrother\\Data";

    public static async Task Main(string[] args)
    {
        await new BigBrother(dataFolder).Run();
    }
}
