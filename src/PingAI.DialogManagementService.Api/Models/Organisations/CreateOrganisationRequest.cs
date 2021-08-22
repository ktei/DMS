using System;

namespace PingAI.DialogManagementService.Api.Models.Organisations
{
    public class CreateOrganisationRequest
    {
        public string Name { get; set; }
        public string? Auth0UserId { get; set; }
        public string? Description { get; set; }
    }
}