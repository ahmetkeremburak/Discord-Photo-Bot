using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Discord;
using Discord.Interactions;
using Discord.WebSocket;

using Serilog;
using Handlers;
using Discord.Commands;

public class Program{       
        
     // Program entry point
    public static Task Main(string[] args) => new Program().MainAsync();


    public async Task MainAsync(){
        var config = new ConfigurationBuilder()
        // this will be used more later on
        .SetBasePath(AppContext.BaseDirectory)
        // I chose using json files for my config data as I am familiar with them
        .AddJsonFile("config.json")
        .Build();
            
        using IHost host = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            services
            // Add the configuration to the registered services
            .AddSingleton(config)
            // Add the DiscordSocketClient, along with specifying the GatewayIntents and user caching
            .AddSingleton(x => new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents = Discord.GatewayIntents.All,
                AlwaysDownloadUsers = true
            }))
            // Used for slash commands and their registration with Discord
            .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
            // Required to subscribe to the various client events used in conjunction with Interactions
            .AddSingleton<InteractionHandler>()
            // Adding the prefix Command Service
            .AddSingleton(x => new CommandService())
            // Adding the prefix command handler
            .AddSingleton<PrefixHandler>()
            // Adding the image handler
            .AddSingleton<ImageHandler>())
            .Build();

            await RunAsync(host);
        }

    public async Task RunAsync(IHost host){
        using IServiceScope serviceScope = host.Services.CreateScope();
        IServiceProvider provider = serviceScope.ServiceProvider;

		var _client = provider.GetRequiredService<DiscordSocketClient>();
        var sCommands = provider.GetRequiredService<InteractionService>();
		await provider.GetRequiredService<InteractionHandler>().InitializeAsync();

        var config = provider.GetRequiredService<IConfigurationRoot>();
		var prefixCommands = provider.GetRequiredService<PrefixHandler>();
        prefixCommands.AddModule<Modules.PrefixModule>();
		await prefixCommands.InitializeAsync();

        // Inject your ImageHandler class as a parameter
        var imageHandler = provider.GetRequiredService<ImageHandler>();
        await imageHandler.InitializeAsync();

        // Subscribe to client log events
        _client.Log += async (LogMessage msg) => {Console.WriteLine(msg.Message + "\n" + msg.Exception);};
        // Subscribe to slash command log events
        sCommands.Log += async (LogMessage msg) => {Console.WriteLine(msg.Message);};
		

        _client.Ready += async () =>{
			Console.WriteLine("I'm good to go boss!");
            await sCommands.RegisterCommandsToGuildAsync(UInt64.Parse(config["testGuild"]));
            };


            await _client.LoginAsync(Discord.TokenType.Bot, config["tokens:discord"]);
            await _client.StartAsync();

            await Task.Delay(-1);
        }
	}