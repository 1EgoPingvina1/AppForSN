using DTC.Domain.Entities.Main;

namespace DTC.Application.Interfaces.Repo
{
    public interface IProjectRepository
    {
        void Add(Project project);
        Task<Project?> GetByIdAsync(int projectId);
        Task<IEnumerable<ProjectType>> GetProjectTypeAsync();
        void Update(Project project);
        void DeleteByIdAsync(Project project);
    }

}
