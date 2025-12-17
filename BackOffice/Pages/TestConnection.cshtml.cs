using BackOffice.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace BackOffice.Pages
{
    public class TestConnectionModel : PageModel
    {
        private readonly DbConnection _db;

        public string Message { get; set; }

        public TestConnectionModel(DbConnection db)
        {
            _db = db;
        }

        public void OnGet()
        {
            try
            {
                using IDbConnection conn = _db.CreateConnection();
                conn.Open();
                Message = "Connexion SQL OK ✔️";
            }
            catch (Exception ex)
            {
                Message = "Erreur SQL ❌ : " + ex.Message;
            }
        }
    }
}
