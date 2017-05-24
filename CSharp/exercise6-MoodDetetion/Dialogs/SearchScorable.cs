﻿namespace Exercise6.Dialogs
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Exercise6.Services;
    using Exercise6.Util;
    using Microsoft.Bot.Builder.Scorables;
    using Microsoft.Bot.Connector;

    public class SearchScorable : IScorable<IActivity, double>
    {
        private const string TRIGGER = "search about ";
        private readonly AzureSearchService searchService = new AzureSearchService();

        public Task DoneAsync(IActivity item, object state, CancellationToken token)
        {
            return Task.CompletedTask;
        }

        public double GetScore(IActivity item, object state)
        {
            bool matched = state != null;
            var score = matched ? 1.0 : double.NaN;
            return score;
        }

        public bool HasScore(IActivity item, object state)
        {
            return state != null;
        }

        public async Task PostAsync(IActivity item, object state, CancellationToken token)
        {
            var message = item as IMessageActivity;

            if (state != null && message != null)
            {
                var text = state.ToString();
                ConnectorClient connector = new ConnectorClient(new Uri(message.ServiceUrl));

                var searchResult = await this.searchService.Search(text);

                Activity replyActivity = ((Activity)message).CreateReply();
                await CardUtil.ShowSearchResults(replyActivity, searchResult, $"I'm sorry, I did not understand '{text}'.\nType 'help' to know more about me :)");
            }
        }

        public async Task<object> PrepareAsync(IActivity item, CancellationToken token)
        {
            var message = item as IMessageActivity;

            if (message != null && !string.IsNullOrWhiteSpace(message.Text))
            {
                if (message.Text.StartsWith(TRIGGER, StringComparison.InvariantCultureIgnoreCase))
                {
                    return message.Text.Substring(TRIGGER.Length);
                }
            }

            return null;
        }
    }
}