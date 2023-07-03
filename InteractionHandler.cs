using System;
using System.Reflection;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;

namespace Handlers
{
    public class InteractionHandler{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _commands;
    private readonly IServiceProvider _services;
    // Construction injection is used to get DiscordSocketClient and Service container
    public InteractionHandler(DiscordSocketClient client, InteractionService commands, IServiceProvider services){
        _client = client;
        _commands = commands;
        _services = services;
    }

    // Will be used to add slash command modules
    // Subscribes to "interaction created event"  
    public async Task InitializeAsync(){
        //add module
        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(),_services);
        //Interaction created event
        _client.InteractionCreated += HandleInteraction;
    }

    // InteractionCreated
    private async Task HandleInteraction(SocketInteraction arg){
        try
        {
            var ctx = new SocketInteractionContext(_client, arg);
            await _commands.ExecuteCommandAsync(ctx, _services);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}
}
