using System;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Users
{
    public class UserListItemDto
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Auth0Id { get; set; }
        public string CreatedAt { get; set; }

        public UserListItemDto(User user)
        {
            UserId = user.Id.ToString();
            Name = user.Name;
            Auth0Id = user.Auth0Id;
            CreatedAt = user.CreatedAt.ToString("o");
        }

        public UserListItemDto()
        {
            
        }
    }
}