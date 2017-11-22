using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using CoinBot.Entity;
using CoinBot.Modules;

namespace CoinBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private const string PortfolioOpts = "My Portfolio";
        private const string AddCoinOpts = "Add Coins";
        private const string RemoveCoinOpts = "Remove Coins";
        private const string AddAlertOpts = "Add Alert";
        
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            if (!string.IsNullOrWhiteSpace(activity.Text))
            {
                if (activity.Text.Equals("Alert", StringComparison.InvariantCultureIgnoreCase) ||
                    activity.Text.Equals(AddAlertOpts, StringComparison.InvariantCultureIgnoreCase))
                {
                    context.Call(new AddAlertDialog(), this.ResumeAfterAddAlertDialog);
                    return;
                }
                if (activity.Text.Equals("Add", StringComparison.InvariantCultureIgnoreCase) ||
                    activity.Text.Equals(AddCoinOpts, StringComparison.InvariantCultureIgnoreCase) ||
                    activity.Text.Equals(AddCoinOpts + 's', StringComparison.InvariantCultureIgnoreCase))
                {
                    context.Call(new AddCoinDialog(), this.ResumeAfterAddCoinDialog);
                    return;
                }
                if (activity.Text.Equals("Remove", StringComparison.InvariantCultureIgnoreCase) ||
                    activity.Text.Equals(RemoveCoinOpts, StringComparison.InvariantCultureIgnoreCase) ||
                    activity.Text.Equals(RemoveCoinOpts + 's', StringComparison.InvariantCultureIgnoreCase))
                {
                    context.Call(new RemoveCoinDialog(), this.ResumeAfterRemoveCoinDialog);
                    return;
                }
                if (activity.Text.Equals("Portfolio", StringComparison.InvariantCultureIgnoreCase) ||
                    activity.Text.Equals(PortfolioOpts, StringComparison.InvariantCultureIgnoreCase) ||
                    activity.Text.Equals("Show portfolio", StringComparison.InvariantCultureIgnoreCase))
                {
                    var data = context.UserData.GetValueOrDefault<UserData>(context.Activity.From.Id);
                    if (data != null)
                        await context.PostAsync($"Your portfolio contains: {await data.Portfolio.ToUSD():C}");
                    else
                        await context.PostAsync($"Your portfolio contains: {0:C}");
                    return;
                }
                // everything else shows options dialog
                this.ShowOptions(context);
            }
        }

        private void ShowOptions(IDialogContext context)
        {
            PromptDialog.Choice(context, 
                this.OnOptionsDialog, 
                new List<string> { PortfolioOpts, AddCoinOpts, RemoveCoinOpts, AddAlertOpts },
                "Choose an option to perform from the list below:",
                "Invalid option set", 
                3);
        }

        private async Task OnOptionsDialog(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string optionSelected = await result;
                switch (optionSelected)
                {
                    case PortfolioOpts:
                        var data = context.UserData.GetValueOrDefault<UserData>(context.Activity.From.Id);
                        if (data != null)
                            await context.PostAsync($"Your portfolio contains: {await data.Portfolio.ToUSD():C}");
                        else
                            await context.PostAsync($"Your portfolio contains: {0:C}");
                        break;
                    case AddCoinOpts:
                        context.Call(new AddCoinDialog(), this.ResumeAfterAddCoinDialog);
                        break;
                    case RemoveCoinOpts:
                        context.Call(new RemoveCoinDialog(), this.ResumeAfterRemoveCoinDialog);
                        break;
                    case AddAlertOpts:
                        context.Call(new AddAlertDialog(), this.ResumeAfterAddAlertDialog);
                        break;
                }
            }
            catch (TooManyAttemptsException)
            {
                await context.PostAsync("Too many attempts :( Let's start over.");
            }
            catch(Exception ex)
            {
                await context.PostAsync($"Failed with message: {ex.Message}");
            }
        }

        private async Task ResumeAfterAddAlertDialog(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var message = await result;

                await context.PostAsync(message);
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync(ex.Message);
            }
            catch (Exception ex)
            {
                await context.PostAsync($"Failed with message: {ex.Message}");
            }
            finally
            {
                context.Wait(this.MessageReceivedAsync);
            }
        }

        private async Task ResumeAfterAddCoinDialog(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var message = await result;

                await context.PostAsync(message);
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync(ex.Message);
            }
            catch(Exception ex)
            {
                await context.PostAsync($"Failed with message: {ex.Message}");
            }
            finally
            {
                context.Wait(this.MessageReceivedAsync);
            }
        }

        private async Task ResumeAfterRemoveCoinDialog(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var message = await result;

                await context.PostAsync(message);
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync(ex.Message);
            }
            catch (Exception ex)
            {
                await context.PostAsync($"Failed with message: {ex.Message}");
            }
            finally
            {
                context.Wait(this.MessageReceivedAsync);
            }
        }
    }
}