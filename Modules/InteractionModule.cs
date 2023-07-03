using Discord.Interactions;
using Discord;
public class InteractionModule : InteractionModuleBase<SocketInteractionContext>{
    [SlashCommand("hello", "Receive a greeting message!")]
    public async Task HandleHelloCommand(){
        await RespondAsync("Hello gov'nah");
    }
}