using DTC.Application.Interfaces.Repo;

namespace DTC.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IProjectRepository ProjectRepository { get; }
        IAuthorRepository AuthorsRepository { get; }
        IAuthorGroupRepository AuthorGroupsRepository { get; }
        Task<int> SaveChangesAsync();
    }
}
