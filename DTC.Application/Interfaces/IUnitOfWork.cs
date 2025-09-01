using DTC.Application.Interfaces.Repo;
using Microsoft.EntityFrameworkCore.Storage;

namespace DTC.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IProjectRepository ProjectRepository { get; }
        IAuthorRepository AuthorsRepository { get; }
        IAuthorGroupRepository AuthorGroupsRepository { get; }

        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackAsync();
        Task<int> SaveChangesAsync();
        IExecutionStrategy GetExecutionStrategy();

    }
}
