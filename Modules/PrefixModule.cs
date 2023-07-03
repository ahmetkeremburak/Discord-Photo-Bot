using Discord;
using Discord.Commands;

namespace Modules
{
    public class PrefixModule : ModuleBase<SocketCommandContext>{
    [Command("hello")]

    public async Task HandleHello(){
        await Context.Message.ReplyAsync("Hello gov'nah");
    }
}
}
