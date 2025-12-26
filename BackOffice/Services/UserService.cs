using BackOffice.Data.Repositories;
using BackOffice.Models;

namespace BackOffice.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;

        public UserService(IUserRepository repo)
        {
            _repo = repo;
        }

        public Task UpdateUserRoleAsync(User user) => _repo.UpdateUserRole(user);

        public Task<int> GetUserCountAsync() => _repo.GetCountAsync();

        public Task<List<User>> GetUsersAsync(string index, int page, int pageSize)
            =>  _repo.GetUsersAsync(index, page, pageSize);
    }
}
