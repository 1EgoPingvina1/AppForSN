using DTC.Application.Interfaces.Repo;
using DTC.Domain.Entities.Main;
using DTC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DTC.Infrastructure.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly ApplicationDataBaseContext _context;

        public AuthorRepository(ApplicationDataBaseContext context) => _context = context;

        public async Task<Author?> GetByUserIdAsync(int userId)
        {
            return await _context.Authors.FirstOrDefaultAsync(a => a.UserId == userId);
        }



        public void Add(Author author)
        {
            _context.Authors.Add(author);
        }
    }
}
