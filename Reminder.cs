using Discord;
using System.Globalization;
using System.Text.RegularExpressions;

internal class Reminder
{
    private readonly static Regex parsingRegex = new Regex("([0-9]{2}/[0-9]{2}/[0-9]{4} [0-9]{2}:[0-9]{2}:[0-9]{2})\n([0-9]+)\n([0-9]+)\n(.+)");

    public ulong ReminderId;
    public DateTime DateTime;
    public ulong UserId;
    public ulong ChannelId;
    public string Text;

    public static Reminder? Load(string folderPath, string filePath)
    {
        string text;
        using (StreamReader streamReader = new StreamReader(Path.Combine(folderPath, filePath)))
            text = streamReader.ReadToEnd();

        if (!parsingRegex.IsMatch(text))
            return null;

        // TODO this is not the way it should be done
        try
        {
            // TODO TryParse may avoid some errors
            GroupCollection data = parsingRegex.Match(text).Groups;
            ulong reminderId = ulong.Parse(filePath);
            DateTime dateTime = DateTime.Parse(data[1].Value);
            ulong userId = ulong.Parse(data[2].Value);
            ulong channelId = ulong.Parse(data[3].Value);
            return new Reminder(dateTime, userId, channelId, data[4].Value, reminderId);
        }
        catch
        {
            return null;
        }
    }

    public Reminder(DateTime dateTime, ulong userId, ulong channelId, string text, ulong reminderId)
    {
        ReminderId = reminderId;
        DateTime = dateTime;
        UserId = userId;
        ChannelId = channelId;
        Text = text;
    }

    public override string ToString()
    {
        return $"> {ReminderId}. {DateTime.ToString("G", CultureInfo.GetCultureInfo("fr-FR"))}: {Text}";
    }

    

    // Return this object under a format adapted to save it in a file
    private string ToText()
    {
        // TODO some comments would be nice
        return $"{DateTime.ToString("G", CultureInfo.GetCultureInfo("fr-FR"))}\n{UserId}\n{ChannelId}\n{Text}";
    }

    public void Save(string folderPath)
    {
        string fileName = Path.Combine(folderPath, ReminderId.ToString());
        if (File.Exists(fileName))
            throw new Exception("File already exists");
        using (StreamWriter streamWriter = new StreamWriter(fileName))
            streamWriter.WriteLine(ToText());
    }
}

internal partial class BigBrother
{
    private const string REMINDERFOLDER = "Reminders";
    //private const string REMINDERFILE = "Reminders.csv";

    private List<Reminder> reminders;

    private void InitRemindMe()
    {
        LoadReminders();
        commands.Add(new Command("remindme", " (?:([0-9]+)d)? ?(?:([0-9]+)h)? ?(?:([0-9]+)m)? (.+)", " <duration> <text>` -> Wait `duration` before sending you back `text`", RemindMe));
        commands.Add(new Command("reminderlist", "` -> Display all of your upcoming reminders", SeeReminders));
        // TODO add command to remove reminders
        commands.Add(new Command("removereminder", " ([0-9]+)", " <Id>` -> Delete the reminder with the given Id", RemoveReminder));

        // Run remind without awaiting so that it doesn't stop the main thread
        Remind();
        onReady += Remind;
    }

    private ulong GetNewId()
    {
        ulong newId = 0;
        // Search for the first available id
        while (reminders.Any(reminder => reminder.ReminderId == newId))
            newId++;
        return newId;
    }

    private void LoadReminders()
    {
        reminders = new List<Reminder>();
        foreach (string path in Directory.GetFiles(GetPath(REMINDERFOLDER)))
        {
            Reminder? newReminder = Reminder.Load(GetPath(REMINDERFOLDER), Path.GetFileName(path));
            if (newReminder != null)
                reminders.Add(newReminder);
        }
    }

    private void DeleteReminder(Reminder reminder)
    {
        reminders.Remove(reminder);
        File.Delete(GetPath(REMINDERFOLDER, reminder.ReminderId.ToString()));
    }

    private async Task RemindMe(IMessage message, GroupCollection args)
    {
        DateTime reminderDate = DateTime.Now;
        if (args[1].Value != "")
        {
            double days;
            if (!double.TryParse(args[1].Value, out days) || days > 365000)
            {
                await SendMessage(message.Channel, "No need, you'll be dead by then");
                return;
            }
            reminderDate = reminderDate.AddDays(days);
        }
        if (args[2].Value != "")
        {
            double hours;
            if (!double.TryParse(args[2].Value, out hours))
            {
                await SendMessage(message.Channel, "Invalid duration");
                return;
            }
            reminderDate = reminderDate.AddHours(hours);
        }
        if (args[3].Value != "")
        {
            double minutes;
            if (!double.TryParse(args[3].Value, out minutes))
            {
                await SendMessage(message.Channel, "Invalid duration");
                return;
            }
            reminderDate = reminderDate.AddMinutes(minutes);
        }

        Reminder newReminder = new Reminder(reminderDate, message.Author.Id, message.Channel.Id, args[4].Value, GetNewId());
        reminders.Add(newReminder);
        newReminder.Save(GetPath(REMINDERFOLDER));
        await SendMessage(message.Channel, $"{{{message.Author.Id}}}: Reminder added for the {reminderDate.ToString("G", CultureInfo.GetCultureInfo("fr-FR"))}");
    }

    private async Task SeeReminders(IMessage message, GroupCollection args)
    {
        string reminderString = "";

        foreach (Reminder reminder in reminders)
            if (reminder.UserId == message.Author.Id)
                reminderString += $"{reminder}\n";

        await SendMessage(message.Channel, reminderString);
    }

    private async Task RemoveReminder(IMessage message, GroupCollection args)
    {
        ulong id;
        if (!ulong.TryParse(args[1].Value, out id))
        {
            await SendMessage(message.Channel, "Invalid id");
            return;
        }

        foreach (Reminder reminder in reminders)
            if (id == reminder.ReminderId)
            {
                if (message.Author.Id != reminder.UserId)
                    await SendMessage(message.Channel, "You cannot remove a reminder that belongs to someone else");
                else
                {
                    DeleteReminder(reminder);
                    await SendMessage(message.Channel, "The reminder has been removed");
                }
                return;
            }

        await SendMessage(message.Channel, "There is no reminder with this id");
    }

    private void Remind() { Remind(60000); }

    private async void Remind(int delay)
    {
        // TODO this method do not work for private messages

        while (IsRunning)
        {
            // Wait for the bot to be ready (In case of disconnection)
            while (!IsReady) { await Task.Delay(delay);  }

            // List of all reminders that have been reminded and must be removed
            List<Reminder> reminded = new List<Reminder>();

            foreach (Reminder reminder in reminders)
            {
                if (reminder.DateTime <= DateTime.Now)
                {
                    await SendMessage(reminder.ChannelId, $"{{{reminder.UserId}}}, I have to remind you:\n> {reminder.Text}");
                    reminded.Add(reminder);
                }
            }

            foreach (Reminder reminder in reminded)
            {
                DeleteReminder(reminder);
            }

            // Wait delay before the next reminders
            await Task.Delay(delay);
        }
    }
}
