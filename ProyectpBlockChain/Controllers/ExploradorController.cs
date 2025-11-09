using Microsoft.AspNetCore.Mvc;

namespace ProyectoBlockChain.Web.Controllers
{
    public class ExploradorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
