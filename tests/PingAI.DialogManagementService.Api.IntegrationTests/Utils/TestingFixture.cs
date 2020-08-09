using System;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.IntegrationTests.Utils
{
    public class TestingFixture
    {
        public Organisation Organisation { get; set; }
        public Project Project { get; set; }
        public Guid UserId { get; set; }
        // public EntityType EntityType { get; set; }
        // public EntityName EntityName { get; set; }
    }
}