using DTC.Application.DTO;
using DTC.Application.Interfaces;
using DTC.Domain.Entities.Main;
using DTC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DTC.Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly IGenericRepository<Project> _genericRepository;
        private readonly ApplicationDataBaseContext _context;

        public ProjectRepository(ApplicationDataBaseContext context, IGenericRepository<Project> genericRepository)
        {
            _context = context;
            _genericRepository = genericRepository;
        }

        public async Task<Project> CreateProjectAsync(Project project)
        {
            await _genericRepository.AddAsync(project);
            await _genericRepository.SaveAsync();
            return project;
        }

        public async Task<bool> DeleteProjectAsync(int projectId)
        {
            await _genericRepository.DeleteAsync(projectId);
            await _genericRepository.SaveAsync();
            return true;
        }

        public async Task<Project?> GetByIdAsync(int projectId)
        {
            return await _genericRepository.GetByIdAsync(projectId);
        }

        public async Task<IEnumerable<Project>> GetProjectsByUserAsync(int userId)
        {
            return await _context.Projects.Where(p => p.CreaterId == userId).ToListAsync();
        }

        public async Task<ProjectStatus?> GetRegisterStatus() => await _context.Statuses.FirstOrDefaultAsync(s => s.Name == "Installed");

        public async Task<Project> UpdateProjectAsync(Project project)
        {
            _genericRepository.Update(project);
            await _genericRepository.SaveAsync();
            return project;
        }
    }
}
