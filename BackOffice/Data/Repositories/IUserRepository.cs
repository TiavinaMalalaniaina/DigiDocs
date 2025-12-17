using BackOffice.Models;

namespace BackOffice.Data.Repositories
{
    public interface IUserRepository
    {
        User Login(string email, string password);
    }
}
