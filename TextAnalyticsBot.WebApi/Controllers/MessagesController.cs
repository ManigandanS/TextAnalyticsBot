﻿using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using TextAnalyticsBot.Api.Luis;
using TextAnalyticsBot.DataModel;
using TextAnalyticsBot.DataModel.Feedback;

namespace TextAnalyticsBot.WebApi
{
    /// <summary>
    /// Class MessagesController.
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [BotAuthentication()]
    public class MessagesController : ApiController
    {
        private string BaseUrl = ConfigurationManager.AppSettings["BaseUrl"];
        private string AccountKey = ConfigurationManager.AppSettings["AccountKey"];
        private int NumLanguages = Convert.ToInt32(ConfigurationManager.AppSettings["NumLanguages"]);

        private static IForm<Feedback> BuildForm()
        {
            var builder = new FormBuilder<Feedback>();

            return builder
                .Field(nameof(Feedback.Country))
                .Field(nameof(Feedback.Event))
                .Field(nameof(Feedback.DateTime))
                .Build();
        }

        internal static IDialog<Feedback> MakeRoot()
        {
            return Chain.From(() => new SubmitFeedbackDialog(BuildForm)).Do(async (context, feedback) =>
            {
                try
                {
                    var completed = await feedback;

                    // Actually write to database
                    await context.PostAsync("Processed your feedback!");
                }
                catch (FormCanceledException<Feedback> e)
                {
                    string reply;
                    if (e.InnerException == null)
                    {
                        reply = $"You quit on {e.Last}--maybe you can finish next time!";
                    }
                    else
                    {
                        reply = "Sorry, I've had a short circuit.  Please try again.";
                    }
                    await context.PostAsync(reply);
                }
            });
        }

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<Message> Post([FromBody]Message message)
        {
            if (message.Type == "Message")
            {
                return await Conversation.SendAsync(message, MakeRoot);
            }
            else
            {
                return HandleSystemMessage(message);
            }
        }

        private string FormatResultMessage(Dictionary<TextAnalyticsResultType, TextAnalyticsResult> input)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat($"Language : {input[TextAnalyticsResultType.Languages].Documents.FirstOrDefault().DetectedLanguages.FirstOrDefault().Name} {Environment.NewLine}");

            List<string> keyPhrases = input[TextAnalyticsResultType.KeyPhrases].Documents.FirstOrDefault().KeyPhrases.Where(s => !string.IsNullOrEmpty(s)).ToList();
            if (keyPhrases != null && keyPhrases.Count > 0)
            {
                sb.AppendFormat($"Key Phrases are : { string.Join(",", input[TextAnalyticsResultType.KeyPhrases].Documents.FirstOrDefault().KeyPhrases)} {Environment.NewLine}");
            }

            sb.AppendFormat($"Sentiment is {input[TextAnalyticsResultType.Sentiment].Documents.FirstOrDefault().Score} {Environment.NewLine}");
            return sb.ToString();
        }

        private Message HandleSystemMessage(Message message)
        {
            if (message.Type == "Ping")
            {
                Message reply = message.CreateReplyMessage("You just pinged me.");
                reply.Type = message.Type;
                return reply;
            }
            else if (message.Type == "DeleteUserData")
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == "BotAddedToConversation")
            {
                Message reply = message.CreateReplyMessage("Hello, I am Text Analytics Bot.");
                reply.Type = message.Type;
                return reply;
            }
            else if (message.Type == "BotRemovedFromConversation")
            {
            }
            else if (message.Type == "UserAddedToConversation")
            {
                Message reply = message.CreateReplyMessage("Hi! I'm Text Analytics Bot. Say \"hi\" if you'd like to chat.");
                reply.Type = message.Type;
                return reply;
            }
            else if (message.Type == "UserRemovedFromConversation")
            {
            }
            else if (message.Type == "EndOfConversation")
            {
                Message reply = message.CreateReplyMessage("Good bye. Hope you had a good time.");
                reply.Type = message.Type;
                return reply;
            }

            return null;
        }
    }
}