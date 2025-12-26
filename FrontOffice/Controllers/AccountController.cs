using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using FrontOffice.Data.Repositories;
using FrontOffice.Models;


public class AccountController : Controller
{
    private readonly UserRepository _repo;

    public AccountController(UserRepository repo)
    {
        _repo = repo;
    }

    // ===== LOGIN =====
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(LoginViewModel model)
    {
        var user = _repo.GetByUsername(model.Username);

        if (user == null ||
            user.PasswordHash != UserRepository.HashPassword(model.Password))
        {
            model.Error = "Nom d'utilisateur ou mot de passe incorrect";
            return View(model);
        }

        // HttpContext.Session.SetString("Username", user.Username);
        // HttpContext.Session.SetString("Role", user.Role);
        HttpContext.Session.SetInt32("UserId", user.Id);
        HttpContext.Session.SetString("Username", user.Username);
        HttpContext.Session.SetString("Role", user.Role);


        return RedirectToAction("Index", "Documents");
    }

    // ===== REGISTER =====
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(RegisterViewModel model)
    {
        var hash = UserRepository.HashPassword(model.Password);

        // var user = new User
        // {
        //     Id = new Random().Next(1000, 9999),
        //     Username = model.Username,
        //     Email = model.Email,
        //     PasswordHash = hash,
        //     Role = "standard"
        // };

        var user = new User
        {
            Username = model.Username,
            Email = model.Email,
            PasswordHash = hash,
            Role = "standard"
        };


        _repo.Create(user);

        return RedirectToAction("Login");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
