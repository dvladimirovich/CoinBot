using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using System.Text.RegularExpressions;
using CoinBot.Entity;
using CoinBot.Modules;
using Newtonsoft.Json;

namespace CoinBot.Dialogs
{
    [Serializable]
    public class AddCoinDialog : IDialog<string>
    {
        private int attempts;
        private string pattern;
        private Regex regex;

        public AddCoinDialog()
        {
            attempts = 3;
            pattern = @"^(?<command>add)\s+(?<amount>\d{1,16}(?:\.\d{1,16})?)\s+(?<coin>(?:\w+(?:\-?\s?\w+(?:\-?\s?\w+)?)?))$";
            regex = new Regex(pattern, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        }

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("In order to add coins, please use command: \"Add 1.0 Bitcoin\" or \"Add 1.0 BTC\"");

            context.Wait(this.MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            if (attempts > 0)
            {
                attempts--;
                var message = await result;
                Match m;
                if (message.Text != null &&
                    (m = regex.Match(message.Text.Trim())).Success)
                {
                    Coin coin = await new CoinApiHandler().GetCoin(m.Groups["coin"].Value);
                    if (coin != null)
                    {
                        var data = context.UserData.GetValueOrDefault<UserData>(context.Activity.From.Id);
                        if (data == null)
                        {
                            data = new UserData(context.Activity.From.Id, context.Activity.From.Name);
                        }
                        data.AddCoins(coin, Convert.ToDecimal(m.Groups["amount"].Value));
                        context.UserData.SetValue(context.Activity.From.Id, data);

                        context.Done($"You've recently added {m.Groups["amount"].Value} {m.Groups["coin"].Value}");
                    }
                    else
                    {
                        await context.PostAsync("No such currency found.");
                        context.Wait(this.MessageReceivedAsync);
                    }
                }
                else
                {
                    await context.PostAsync("I'm sorry, invalid command or the value you specified isn't correct. Please, try again.");
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