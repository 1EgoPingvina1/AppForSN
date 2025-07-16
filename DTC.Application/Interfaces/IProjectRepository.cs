using DTC.Domain.Entities.Main;

namespace DTC.Application.Interfaces
{
    public interface IProjectRepository
    {
        Task<Project> CreateProjectAsync(Project project);
        Task<IEnumerable<Project>> GetProjectsByUserAsync(int userId);
        Task<ProjectStatus?> GetRegisterStatus();
        Task<Project?> GetByIdAsync(int projectId);
        Task<Project> UpdateProjectAsync(Project project);
        Task<bool> DeleteProjectAsync(int projectId);
    }

}
