using BackOffice.Models;

namespace BackOffice.Services
{
    public interface IAuthService
    {
        public User Login(string email, string password);
    }
}
