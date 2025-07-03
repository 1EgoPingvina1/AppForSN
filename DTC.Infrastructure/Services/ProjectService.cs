using DTC.Application.DTO;
using DTC.Application.Interfaces;
using DTC.Domain.Entities.Main;

namespace DTC.Infrastructure.Services
{
    public class ProjectService
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectService(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<Project> CreateProjectAsync(Project project)
        {
            return await _projectRepository.CreateProjectAsync(project);
        }
    }
}
