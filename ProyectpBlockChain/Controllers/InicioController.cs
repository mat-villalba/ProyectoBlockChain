    using Microsoft.AspNetCore.Mvc;

namespace ProyectoBlockChain.Web.Controllers
{
    public class InicioController : Controller
    {
        // lobby principal, una vez que el usuario ha iniciado sesión/registrado
        public IActionResult Index()
        {
            string nombreUsuario = HttpContext.Session.GetString("UserName");
            string walletUsuario = HttpContext.Session.GetString("UserWalletAddress");

            if (string.IsNullOrEmpty(walletUsuario) || string.IsNullOrEmpty(nombreUsuario))
            {
                TempData["Error"] = "Por favor, inicia sesión para continuar.";
                return RedirectToAction("IniciarSesion", "Jugador");
            }
            ViewData["NombreUsuario"] = nombreUsuario;
            ViewData["WalletUsuario"] = walletUsuario;

            return View(); 
        }
    }
}
