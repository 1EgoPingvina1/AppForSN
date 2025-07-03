using DTC.Application.DTO;
using DTC.Application.Interfaces;
using DTC.Domain.Entities.Main;
using DTC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DTC.Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ApplicationDataBaseContext _context;

        public ProjectRepository(ApplicationDataBaseContext context)
        {
            _context = context;
        }

        public async Task<Project> CreateProjectAsync(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return project;
        }

        public async Task<IEnumerable<Project>> GetProjectsByUserAsync(int userId)
        {
            return await _context.Projects
                .Where(p => p.CreaterId == userId)
                .ToListAsync();
        }

        public async Task<ProjectStatus?> GetRegisterStatus() => await _context.Statuses.FirstOrDefaultAsync(s => s.Name == "Installed");
    }
}
