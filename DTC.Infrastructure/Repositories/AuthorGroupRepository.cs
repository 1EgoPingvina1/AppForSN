using DTC.Application.ErrorHandlers;
using DTC.Application.Interfaces.Repo;
using DTC.Domain.Entities.Main;
using DTC.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Security.Claims;

namespace DTC.Infrastructure.Repositories
{
    public class AuthorGroupRepository : IAuthorGroupRepository
    {
        private readonly ApplicationDataBaseContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthorGroupRepository(ApplicationDataBaseContext context)
        {
            _context = context;
            _httpContextAccessor = new HttpContextAccessor();
        }

        public async Task<AuthorGroup?> GetByIdAsync(int id)
        {
            return await _context.AuthorGroups.FindAsync(id);
        }

        public async Task<AuthorGroup?> GetByIdWithMembersAsync(int id)
        {
            return await _context.AuthorGroups
                .Include(g => g.Members)
                .ThenInclude(m => m.Author)
                .ThenInclude(a => a.User) 
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public void Add(AuthorGroup group)
        {
            _context.AuthorGroups.Add(group);
        }
    }
}
