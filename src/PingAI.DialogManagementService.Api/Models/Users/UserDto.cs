using System;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Users
{
    public class UserDto
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Auth0Id { get; set; }
        public DateTime CreatedAt { get; set; }

        public UserDto(User user)
        {
            UserId = user.Id.ToString();
            Name = user.Name;
            Auth0Id = user.Auth0Id;
            CreatedAt = user.CreatedAt;
        }

        public UserDto()
        {
            
        }
    }
}