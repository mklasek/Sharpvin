using Discord;
using Discord.WebSocket;

namespace Sharpvin
{
    public class Program
    {
        private const String Token = "MTA2NDkwNjA1ODkwNjIyMjY5Mg.GPMKpN.SxFtmuTLezn4pVVEqnqhHlRyaRPot-QOjJLfec";

        private DiscordSocketClient client;
        private MessageHandler Handler;

        public Program()
        {
            //create client with config
            DiscordSocketConfig config = new DiscordSocketConfig();
            config.GatewayIntents = GatewayIntents.All;
            client = new DiscordSocketClient(config);

            Handler = MessageHandler.GetInstance();
        }

        public static Task Main(string[] args) => new Program().MainAsync();

        public async Task MainAsync()
        {

            //event handlers
            client.Log += Log;
            client.MessageReceived += Handler.OnMessageReceived;
            client.Ready += OnReady;

            //connect to discord
            await client.LoginAsync(TokenType.Bot, Token);
            await client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private Task OnReady()
        {
            if (client.CurrentUser != null)
                Handler.SetID(client.CurrentUser.Id);

            if (Handler != null)
                Handler.OnReady();

            return Task.CompletedTask;
        }
    }
   
}