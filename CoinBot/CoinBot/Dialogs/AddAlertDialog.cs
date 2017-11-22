using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using System.Text.RegularExpressions;
using CoinBot.Entity;
using CoinBot.Modules;

namespace CoinBot.Dialogs
{
    [Serializable]
    public class AddAlertDialog : IDialog<string>
    {
        private int attempts;

        public AddAlertDialog()
        {
            attempts = 3;
        }

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("In order to add alert, please set the percentage value (e.g. '1', '5')");

            context.Wait(this.MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            if (attempts > 0)
            {
                attempts--;
                var message = await result;
                int percent;
                if (Int32.TryParse(message.Text, out percent) && (percent > 0))
                {
                    // TODO: ADD NOTIFIER
                    context.Done($"I will notify you when your portfolio grows by {percent}%.");
                }
                else
                {
                    await context.PostAsync("I'm sorry, value you specified isn't correct. Please, try again.");
                    context.Wait(this.MessageReceivedAsync);
                }
            }
            else
            {
                context.Fail(new TooManyAttemptsException("Too many attempts :( Let's start over."));
            }
        }
    }
}