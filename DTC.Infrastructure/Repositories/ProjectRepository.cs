using DTC.Application.DTO;
using DTC.Application.Interfaces.Repo;
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

        public async Task<Project?> GetByIdAsync(int id)
        {
            return await _context.Projects.FindAsync(id);
        }

        public void Add(Project project)
        {
            _context.Projects.Add(project);
        }

        public void Update(Project project)
        {
            _context.Projects.Update(project);
        }

        public void DeleteByIdAsync(Project project)
        {
            _context.Projects.Remove(project);
        }
    }
}
