using BackOffice.Models;

namespace BackOffice.Services
{
    public interface ISessionService
    {
        void SetUser(User user);
        User? GetUser();
        void Clear();
        bool IsLoggedIn();
    }
}
