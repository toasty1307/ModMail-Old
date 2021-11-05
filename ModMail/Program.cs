namespace ModMail;

public class Program
{
    // main async method of the program
    // creates a instance of ModMailBot
    // and runs the bot
    public static async Task Main()
    {
        var bot = new ModMailBot();
        await bot.RunAsync();
        await Task.Delay(-1);
    }
}
