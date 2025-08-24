using DTC.Domain.Entities.Main;

namespace DTC.Application.Interfaces.Repo
{
    public interface IAuthorRepository
    {
        Task<Author?> GetByUserIdAsync(int userId);
        void Add(Author author);
    }
}
