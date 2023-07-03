using Discord.Interactions;
using Discord;

namespace Modules
{
    public class InteractionModule : InteractionModuleBase<SocketInteractionContext>{
    [SlashCommand("hello", "Receive a greeting message!")]
        public async Task HandleHelloCommand(){
            await RespondAsync("Hello gov'nah");
        }

        [SlashCommand("howtouse","Talks about how to use the Hoarding Bot")]
        public async Task HandleHowtouseCommand(){
            var button = new ButtonBuilder(){
                Label = "Button!",
                CustomId = "button",
                Style = ButtonStyle.Primary
            };
        
            var menu = new SelectMenuBuilder(){
                CustomId = "menu",
                Placeholder = "Sample Menu",
            };

            menu.AddOption("First Option", "first option value");
            menu.AddOption("Second Option", "second option");

            var component = new ComponentBuilder();
            component.WithButton(button);
            component.WithSelectMenu(menu);

            await RespondAsync("testing", components: component.Build());
        }

        [ComponentInteraction("button")]
        public async Task HandleButtonInput(){
            await RespondWithModalAsync<DemoModal>("demo_modal");
        }

        [ComponentInteraction("menu")]

        public async Task HandleMenuInput(string[] inputs){
            await RespondAsync(inputs[0]);
        }
        
        [ModalInteraction("demo_modal")]
        public async Task HandleModalInput(DemoModal modal){
            string input = modal.Greeting;
            await RespondAsync(input);
        }

    }

    public class DemoModal : IModal{
        public string Title => "Demo Modal";
        [InputLabel("Send a greeting!")]
        [ModalTextInput("greeting_input", TextInputStyle.Short, placeholder:"Be nice...", maxLength: 100)]

        public string Greeting {get; set;}
    }
}
