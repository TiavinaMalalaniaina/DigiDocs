using BackOffice.Data.Repositories;
using BackOffice.Models;
using BackOffice.Exceptions;

namespace BackOffice.Services
{
    public class AuthService(IUserRepository repo) : IAuthService
    {
        private readonly IUserRepository _repo = repo;

        public User Login(string email, string password)
        {
            User? user = _repo.Login(email, password) ?? throw new LoginFailedException();
            return user;
        }

    }
}
