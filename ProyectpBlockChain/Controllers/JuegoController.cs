using Microsoft.AspNetCore.Mvc;

namespace ProyectoBlockChain.Web.Controllers
{
    public class JuegoController : Controller
    {
        public IActionResult Jugar()
        {
            return View();
        }
        public IActionResult FinalizarVotacion()
        {
            return View();
        }
        public IActionResult FinalizarJuego()
        {
            return View();
        }
    }
}
