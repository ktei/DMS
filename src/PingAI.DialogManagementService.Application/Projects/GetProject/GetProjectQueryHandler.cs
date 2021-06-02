using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services;
using PingAI.DialogManagementService.Application.Interfaces.Services.Caching;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Model;
using static PingAI.DialogManagementService.Domain.ErrorHandling.ErrorDescriptions;

namespace PingAI.DialogManagementService.Application.Projects.GetProject
{
    public class GetProjectQueryHandler : IRequestHandler<GetProjectQuery, Project>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly ICacheService _cacheService;

        public GetProjectQueryHandler(IProjectRepository projectRepository, IAuthorizationService authorizationService,
            ICacheService cacheService)
        {
            _projectRepository = projectRepository;
            _authorizationService = authorizationService;
            _cacheService = cacheService;
        }

        public async Task<Project> Handle(GetProjectQuery request, CancellationToken cancellationToken)
        {
            var canRead = await _authorizationService.UserCanReadProject(request.ProjectId);
            if (!canRead)
                throw new ForbiddenException(ProjectReadDenied);

            var cachedProject = await _cacheService.GetObject<Project.Cache>(Domain.Model.Project.Cache.MakeKey(request.ProjectId));
            if (cachedProject != null)
                return new Project(cachedProject);
            
            var project = await _projectRepository.GetProjectById(request.ProjectId);
            if (project == null)
            {
                throw new ForbiddenException(ProjectReadDenied);
            }
            
            var cache = new Project.Cache(project);
            await _cacheService.SetObject(Project.Cache.MakeKey(request.ProjectId), cache);

            return project;
        }
    }
}