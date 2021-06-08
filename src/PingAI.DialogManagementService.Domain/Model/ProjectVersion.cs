using System;
using PingAI.DialogManagementService.Domain.Utils;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class ProjectVersion : DomainEntity
    {
        public Guid Id { get; private set; }

        public Guid OrganisationId { get; private set; }

        public Guid ProjectId { get; private set; }

        public Project Project { get; private set; }

        public Guid VersionGroupId { get; private set; }
        
        public ProjectVersionNumber Version { get; private set; }

        public ProjectVersion(Guid projectId, Guid organisationId, Guid versionGroupId, ProjectVersionNumber version)
        {
            if (projectId.IsEmpty())
                throw new ArgumentException($"{nameof(projectId)} cannot be empty.");
            ProjectId = projectId;
            OrganisationId = organisationId;
            VersionGroupId = versionGroupId;
            Version = version;
        }

        public static ProjectVersion NewDesignTime(Project project) =>
            new ProjectVersion(project, project.Id, ProjectVersionNumber.NewDesignTime());

        public static ProjectVersion NewRuntime(Project project, int versionNumber)
        {
            if (versionNumber == ProjectVersionNumber.DesignTime)
                throw new ArgumentException($"Runtime {nameof(versionNumber)} {versionNumber} " +
                                            $"must be greater than {ProjectVersionNumber.DesignTime}.");
            return new ProjectVersion(project, project.Id, new ProjectVersionNumber(versionNumber));
        }


        private ProjectVersion(Project project, Guid versionGroupId, ProjectVersionNumber version)
        {
            Project = project ?? throw new ArgumentNullException(nameof(project));
            OrganisationId = project.OrganisationId;
            VersionGroupId = versionGroupId;
            Version = version;
        }
    }
}
