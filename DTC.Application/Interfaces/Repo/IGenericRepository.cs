namespace DTC.Application.Interfaces.Repo
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        void Update(T entity);
        Task SaveAsync();
        Task DeleteAsync(int id);
    }
}
