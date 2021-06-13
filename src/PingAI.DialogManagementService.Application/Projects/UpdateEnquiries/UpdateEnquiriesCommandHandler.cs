using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services;
using PingAI.DialogManagementService.Application.Interfaces.Services.Security;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Domain.Repositories;
using static PingAI.DialogManagementService.Domain.ErrorHandling.ErrorDescriptions;

namespace PingAI.DialogManagementService.Application.Projects.UpdateEnquiries
{
    public class UpdateEnquiriesCommandHandler : IRequestHandler<UpdateEnquiriesCommand, Project>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthorizationService _authorizationService;

        public UpdateEnquiriesCommandHandler(IProjectRepository projectRepository, IUnitOfWork unitOfWork,
            IAuthorizationService authorizationService)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
            _authorizationService = authorizationService;
        }

        public async Task<Project> Handle(UpdateEnquiriesCommand request, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.FindById(request.ProjectId);
            if (project == null)
                throw new ForbiddenException(ProjectWriteDenied);
            
            var canWrite = await _authorizationService.UserCanWriteProject(request.ProjectId);
            if (!canWrite)
                throw new ForbiddenException(ProjectWriteDenied);

            project.UpdateEnquiries(request.Enquiries);
            await _unitOfWork.SaveChanges();
            return project;
        }
    }
}