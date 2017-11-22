using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Linq;
using System;
using System.Web.Http.Description;

namespace CoinBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        [ResponseType(typeof(void))]
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
            }
            else if (activity.Type == ActivityTypes.Event)
            {
                // TODO: Receive notification from scheduler!
            }
            else
            {
                await HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private async Task<Activity> HandleSystemMessage(Activity activity)
        {
            var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            switch (activity.Type)
            {
                case ActivityTypes.DeleteUserData:
                    await connector.Conversations.ReplyToActivityAsync(
                        activity.CreateReply("All your data will be lost!"));
                    // delete data
                    await activity.GetStateClient().BotState.DeleteStateForUserAsync(activity.ChannelId, activity.From.Id);
                    break;

                case ActivityTypes.ConversationUpdate: // user joins or leaves conversation
                    if (activity.MembersAdded.Any(acc => acc.Id == activity.Recipient.Id))
                    {
                        await connector.Conversations.ReplyToActivityAsync(
                            activity.CreateReply("Hi, User! My name is **CoinBot**. You can *[add coin]*, *[remove coin]*, *see your [portfolio]* or *[add alert]* when to update you."));
                    }
                    else if (activity.MembersRemoved.Any(acc => acc.Id == activity.Recipient.Id))
                    {
                        await connector.Conversations.ReplyToActivityAsync(
                            activity.CreateReply("It's bad that you're gone. All your data will be lost!"));
                        // delete data
                        await activity.GetStateClient().BotState.DeleteStateForUserAsync(activity.ChannelId, activity.From.Id);
                    }
                    break;

                case ActivityTypes.ContactRelationUpdate: // user added or removed bot from his contact list
                    if (activity.AsContactRelationUpdateActivity().Action == ContactRelationUpdateActionTypes.Add)
                    {
                        await connector.Conversations.ReplyToActivityAsync(
                            activity.CreateReply("Hi, User! My name is **CoinBot**. You can *[add coin]*, *[remove coin]*, *see your [portfolio]* or *[add alert]* when to update you."));
                    }
                    else if (activity.AsContactRelationUpdateActivity().Action == ContactRelationUpdateActionTypes.Remove)
                    {
                        await connector.Conversations.ReplyToActivityAsync(
                               activity.CreateReply("It's bad that you're gone. All your data will be lost!"));
                        // delete data
                        await activity.GetStateClient().BotState.DeleteStateForUserAsync(activity.ChannelId, activity.From.Id);
                    }
                    break;

                case ActivityTypes.Typing:
                    break;

                case ActivityTypes.Ping:
                    break;

                default:
                    break;
            }
            return null;
        }

    }
}