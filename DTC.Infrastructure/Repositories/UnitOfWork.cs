using DTC.Application.Interfaces.Repo;
using DTC.Application.Interfaces;
using DTC.Infrastructure.Data;

namespace DTC.Infrastructure.Repositories
{

    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDataBaseContext _context;
        public IAuthorGroupRepository AuthorGroupsRepository { get; private set; }
        public IAuthorRepository AuthorsRepository { get; private set; }
        public IProjectRepository ProjectRepository { get; }

        public UnitOfWork(ApplicationDataBaseContext context)
        {
            _context = context;
            ProjectRepository = new ProjectRepository(_context);
            AuthorGroupsRepository = new AuthorGroupRepository(_context);
            AuthorsRepository = new AuthorRepository(_context);
        }

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
        
    }
}
