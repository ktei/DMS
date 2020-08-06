using System;
using System.Collections.Generic;

namespace PingAI.DialogManagementService.Application.Interfaces.Services.Nlu
{
    public class SaveIntentRequest
    {
        public Guid ProjectId { get; set; }
        public Guid IntentId { get; set; }
        public string Name { get; set; }
        public List<TrainingPhrase> TrainingPhrases { get; set; } = new List<TrainingPhrase>();
    }
}