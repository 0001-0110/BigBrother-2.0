using Discord;

internal partial class Extensions
{
    // Used for debug
    public static string Repr<T>(this T[,] matrix)
    {
        string result = "";
        for (int x = 0; x < matrix.GetLength(1); x++)
        {
            for (int y = 0; y < matrix.GetLength(0); y++)
                result += $"{matrix[y, x]}, ";
            result += Environment.NewLine;
        }
        return result;
    }
}

internal partial class BigBrother
{
    private void InitDebug()
    {
        onReady += async () => logChannel = await client.GetChannelAsync(settings.LogChannelId) as IMessageChannel;
    }

    public void DebugLog(string logMessage)
    {
        Thread thread = new Thread(() => SendDebugLog(logMessage));
        thread.Start();
    }

    private async void SendDebugLog(string log)
    {
        // Wait to be ready to send the logs
        while (!IsReady)
        {
            // TODO this line is dumping every logs that are created while lauching the app if IsRunning is set to false at the start
            if (!IsRunning)
                return;
            await Task.Delay(10000);
        }

        await SendMessage(logChannel, $"```\n{log}```");
    }
}
