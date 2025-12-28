using BackOffice.Models;
using BackOffice.Data;
using BackOffice.Models.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DbConnection _db;
        private readonly DigiDocsDbContext _context;


        public UserRepository(DbConnection db, DigiDocsDbContext context)
        {
            _db = db;
            _context = context;
        }

        public async Task UpdateUserRoleAsync(User user)
        {
            var userEF = await _context.Users.FindAsync(user.Id);

            if (userEF == null)
                return; // ou throw

            userEF.Role = user.Role;

            await _context.SaveChangesAsync();
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.Users.CountAsync();
        }

        public async Task<List<User>> GetUsersAsync(string? index, int page, int pageSize)
        {
            var query = _context.Users.AsQueryable();

            query = query.Where(u => u.Role != UserRole.Admin);

            if (!string.IsNullOrWhiteSpace(index))
            {
                query = query.Where(u =>
                    u.Username.Contains(index) ||
                    u.Email.Contains(index) ||
                    u.Role.ToString().Contains(index)
                );
            }

            return await query
                .OrderBy(u => u.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }


        public User? Login(string email, string password)
        {
            return _context.Users
                .Where(u =>
                    u.Email == email &&
                    u.PasswordHash == password &&
                    u.Role == UserRole.Admin
                )
                .Select(u => new User
                {
                    Id = u.Id,
                    Email = u.Email,
                    Username = u.Username,
                    Role = u.Role
                })
                .FirstOrDefault();
        }

    }
}
