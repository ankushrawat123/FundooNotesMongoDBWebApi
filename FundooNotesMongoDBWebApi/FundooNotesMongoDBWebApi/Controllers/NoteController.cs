using Microsoft.AspNetCore.Mvc;

namespace FundooNotesMongoDBWebApi.Controllers
{
    public class NoteController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
