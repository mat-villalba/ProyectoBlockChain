using Microsoft.AspNetCore.Mvc;

namespace ProyectoBlockChain.Web.Controllers
{
    public class InicioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Home()
        {
            return View();
        }   
    }
}
