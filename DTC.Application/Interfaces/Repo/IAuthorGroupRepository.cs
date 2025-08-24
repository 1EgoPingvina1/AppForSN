using DTC.Domain.Entities.Main;

namespace DTC.Application.Interfaces.Repo
{
    public interface IAuthorGroupRepository
    {
        Task<AuthorGroup?> GetByIdAsync(int id);
        Task<AuthorGroup?> GetByIdWithMembersAsync(int id);
        void Add(AuthorGroup group);
    }
}
