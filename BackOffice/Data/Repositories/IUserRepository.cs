using BackOffice.Models;

namespace BackOffice.Data.Repositories
{
    public interface IUserRepository
    {
        User Login(string email, string password);
        Task<List<User>> GetUsersAsync(string index, int page, int pageSize);
        Task<int> GetCountAsync();
        Task UpdateUserRoleAsync(User user);
    }
}
