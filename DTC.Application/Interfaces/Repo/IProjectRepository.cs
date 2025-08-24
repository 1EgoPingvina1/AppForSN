using DTC.Domain.Entities.Main;

namespace DTC.Application.Interfaces.Repo
{
    public interface IProjectRepository
    {
        void Add(Project project);
        Task<Project?> GetByIdAsync(int projectId);
        void Update(Project project);
        //Task<bool> DeleteProjectAsync(int projectId);
    }

}
