using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using FrontOffice.Data.Repositories;
using FrontOffice.Helpers;
using FrontOffice.Models;


public class DocumentsController : Controller
{
    private readonly DocumentRepository _repo;

    public DocumentsController(DocumentRepository repo)
    {
        _repo = repo;
    }

    // public IActionResult Index()
    // {
    //     if (HttpContext.Session.GetInt32("UserId") == null)
    //     {
    //         return RedirectToAction("Login", "Account");
    //     }

    //     var documents = _repo.GetAll();
    //     ViewBag.Role = HttpContext.Session.GetString("Role");

    //     return View(documents);
    // }

    public IActionResult Download(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var role = HttpContext.Session.GetString("Role");

        if (userId == null || role == null)
            return RedirectToAction("Login", "Account");

        var document = _repo.GetById(id);
        if (document == null)
            return NotFound();

        if (!AccessHelper.CanDownload(role, document.AccessLevel))
            return Forbid();

        _repo.LogDownload(document.Id, userId.Value);
        _repo.IncrementDownloadCount(document.Id);

        return File(document.FileData, document.ContentType, document.FileName);
    }

    // public IActionResult Index(string? category)
    // {
    //     var role = HttpContext.Session.GetString("Role");
    //     if (role == null)
    //         return RedirectToAction("Login", "Auth");

    //     var documents = _documentRepository
    //         .GetAccessibleDocuments(role, category);

    //     ViewBag.SelectedCategory = category;
    //     return View(documents);
    // }

    // public IActionResult Index(string? category)
    // {
    //     var role = HttpContext.Session.GetString("Role");
    //     if (role == null)
    //         return RedirectToAction("Login", "Account");

    //     var documents = _repo.GetAccessibleDocuments(role, category);

    //     ViewBag.Role = role;
    //     ViewBag.SelectedCategory = category;

    //     return View(documents);
    // }




    // public IActionResult Index(string? category, string sort = "date")
    // {
    //     var role = HttpContext.Session.GetString("Role");
    //     if (role == null)
    //         return RedirectToAction("Login", "Account");

    //     var documents = _repo.GetAccessibleDocuments(role, category, sort);

    //     ViewBag.Role = role;
    //     ViewBag.SelectedCategory = category;
    //     ViewBag.CurrentSort = sort;

    //     return View(documents);
    // }



    public IActionResult Index(
    string? category,
    string sort = "date",
    int page = 1)
    {
        var role = HttpContext.Session.GetString("Role");
        if (role == null)
            return RedirectToAction("Login", "Account");

        int pageSize = 5;

        var documents = _repo.GetAccessibleDocuments(
            role,
            category,
            sort,
            page,
            pageSize
        );

        int totalDocuments = _repo.CountAccessibleDocuments(role, category);
        int totalPages = (int)Math.Ceiling(totalDocuments / (double)pageSize);

        ViewBag.Role = role;
        ViewBag.SelectedCategory = category;
        ViewBag.CurrentSort = sort;
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;

        return View(documents);
    }


}
