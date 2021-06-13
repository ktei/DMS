using System.Collections.Generic;
using PingAI.DialogManagementService.Application.Interfaces.Services.Nlu;

namespace PingAI.DialogManagementService.Infrastructure.Services.Nlu
{
    public class TrainingPhrase
    {
        public List<PhrasePart> Parts { get; set; } = new List<PhrasePart>();

    }
}