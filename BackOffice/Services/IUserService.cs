using BackOffice.Models;
namespace BackOffice.Services
{
    public interface IUserService
    {
        Task<int> GetUserCountAsync();
        Task<List<User>> GetUsersAsync(string index, int page, int pageSize);
        Task UpdateUserRoleAsync(User user);
    }
}
