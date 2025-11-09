using DTC.Application.Interfaces.Repo;
using DTC.Application.Interfaces.Services;
using Microsoft.EntityFrameworkCore.Storage;

namespace DTC.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IProjectRepository ProjectRepository { get; }
        IAuthorRepository AuthorsRepository { get; }
        IAuthorGroupRepository AuthorGroupsRepository { get; }
        IAuthService AuthService { get; }
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackAsync();
        Task<int> SaveChangesAsync();
        IExecutionStrategy GetExecutionStrategy();

    }
}
