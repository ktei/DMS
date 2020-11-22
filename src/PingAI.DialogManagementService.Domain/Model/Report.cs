using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class Report
    {
        public string Title { get; }

        public Report(string title)
        {
            Title = title;
        }
        
        private readonly List<UnmatchedPhrase> _unmatchedPhrases = new List<UnmatchedPhrase>();

        public ReadOnlyCollection<UnmatchedPhrase> UnmatchedPhrases =>
            new ReadOnlyCollection<UnmatchedPhrase>(_unmatchedPhrases);

        private readonly List<Dialog> _dialogs = new List<Dialog>();
        
        public ReadOnlyCollection<Dialog> Dialogs => new ReadOnlyCollection<Dialog>(_dialogs);

        public void Build(IEnumerable<ChatHistory> chatHistories)
        {
            BuildFromHistories(chatHistories);
        }

        private void BuildFromHistories(IEnumerable<ChatHistory> chatHistories)
        {
            var requestResponseList = chatHistories.GroupBy(h => h.RequestId);
            foreach (var pair in requestResponseList)
            {
                var request = pair.FirstOrDefault(x => x.Input != null);
                var response = pair.FirstOrDefault(x => x.Output != null);
                
                if (request?.ChatHistoryInput == null || response?.ChatHistoryOutput == null)
                    continue;
                if (request.ChatHistoryInput?.Text == null)
                    continue;

                string? matchedFaq = null;
                if (response.ChatHistoryOutput.MatchingResult != MatchingResult.Matched)
                {
                    _unmatchedPhrases.Add(new UnmatchedPhrase(request.ChatHistoryInput!.Text.Value,
                        response.ChatHistoryOutput!.MatchingResult.ToString(),
                        request.CreatedAt));
                }
                else
                {
                    matchedFaq = response.ChatHistoryOutput.Intent!.Name;
                }

                _dialogs.Add(new Dialog(request.ChatHistoryInput.Text.Value,
                    matchedFaq, response.ChatHistoryOutput.MatchingResult, response.CreatedAt));
            }
        }
    }

    public class Dialog
    {
        public string UserPhrases { get; }
        public string? MatchedFaq { get; }
        public MatchingResult MatchingResult { get; }
        public DateTime Timestamp { get; }

        public Dialog(string userPhrases, string? matchedFaq, MatchingResult matchingResult,
            DateTime timestamp)
        {
            UserPhrases = userPhrases;
            MatchedFaq = matchedFaq;
            MatchingResult = matchingResult;
            Timestamp = timestamp;
        }
    }

    public class UnmatchedPhrase
    {
        public string Phrase { get; }
        public string Reason { get; }
        public DateTime Timestamp { get; }

        public UnmatchedPhrase(string phrase, string reason, DateTime timestamp)
        {
            Phrase = phrase;
            Reason = reason;
            Timestamp = timestamp;
        }
    }
}
