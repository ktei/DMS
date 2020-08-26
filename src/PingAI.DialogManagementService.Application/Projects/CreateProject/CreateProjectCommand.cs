using System;
using MediatR;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Projects.CreateProject
{
    public class CreateProjectCommand : IRequest<Project>
    {
        public string Name { get; set; }

        public CreateProjectCommand(string name)
        {
            Name = name;
        }

        public CreateProjectCommand()
        {
            
        }
    }
}