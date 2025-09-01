using DTC.Application.Interfaces.Repo;
using DTC.Application.Interfaces;
using DTC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace DTC.Infrastructure.Repositories
{

    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDataBaseContext _context;
        private IDbContextTransaction _transaction;
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

        public void Dispose() => _transaction?.Dispose();

        public async Task BeginTransactionAsync() => _transaction = await _context.Database.BeginTransactionAsync();

        public async Task CommitTransactionAsync()
        {
            if(_transaction is not null)
            {
                await _context.SaveChangesAsync();
                await _transaction.CommitAsync();
            }
        }
        public async Task RollbackAsync()
        {
            if(_transaction is not null)
            {
                await _transaction.RollbackAsync();
            }
        }

        public IExecutionStrategy GetExecutionStrategy() => _context.Database.CreateExecutionStrategy();
    }
}
