using BackOffice.Models;
using System.Text.Json;

namespace BackOffice.Services
{
    public class SessionService(IHttpContextAccessor httpContextAccessor) : ISessionService
    {
        public void SetUser(User user)
        {
            var json = JsonSerializer.Serialize(user);
            httpContextAccessor.HttpContext?.Session.SetString("User", json);
        }

        public User? GetUser()
        {
            var json = httpContextAccessor.HttpContext?.Session.GetString("User");
            return string.IsNullOrEmpty(json) ? null : JsonSerializer.Deserialize<User>(json);
        }

        public void Clear()
        {
            httpContextAccessor.HttpContext?.Session.Clear();
        }
        public bool IsLoggedIn()
        {
            return GetUser() != null;
        }
    }
}
