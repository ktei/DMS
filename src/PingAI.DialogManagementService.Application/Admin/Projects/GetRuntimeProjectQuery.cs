using System;
using MediatR;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Admin.Projects
{
    public class GetRuntimeProjectQuery : IRequest<Project>
    {
        public Guid DesignTimeProjectId { get; set; }

        public GetRuntimeProjectQuery(Guid designTimeProjectId)
        {
            DesignTimeProjectId = designTimeProjectId;
        }

        public GetRuntimeProjectQuery()
        {
            
        }
    }
}