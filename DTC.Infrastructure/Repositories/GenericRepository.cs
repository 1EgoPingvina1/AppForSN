using DTC.Application.Interfaces;
using DTC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DTC.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDataBaseContext _context;
        private readonly DbSet<T> _data;

        public GenericRepository(ApplicationDataBaseContext context)
        {
            _context = context;
            _data = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _data.ToListAsync();
        public async Task<T?> GetByIdAsync(int id) => await _data.FindAsync(id);
        public async Task AddAsync(T entity) => await _data.AddAsync(entity);
        public void Update(T entity) => _context.Entry(entity).State = EntityState.Modified;
        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null) _data.Remove(entity);
        }
        public async Task SaveAsync() => await _context.SaveChangesAsync();
    }
}
