public static class Program
{
    private static string dataFolder = "..\\..\\..\\Data";

    public static async Task Main(string[] args)
    {
        await new BigBrother(dataFolder).Run();
    }
}
