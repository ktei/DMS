using System.Collections.Generic;

namespace PingAI.DialogManagementService.Application.Interfaces.Services.Nlu
{
    public class TrainingPhrase
    {
        public List<PhrasePart> Parts { get; set; } = new List<PhrasePart>();

    }
}