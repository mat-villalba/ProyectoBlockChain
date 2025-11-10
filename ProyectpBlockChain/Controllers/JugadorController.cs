using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProyectoBlockChain.Data.Data;
using ProyectoBlockChain.Logica.Interfaces;
using ProyectoBlockChain.Web.Models;

namespace ProyectoBlockChain.Web.Controllers
{
    public class JugadorController : Controller
    {
        private readonly ILogicaJugador _jugadorLogica;

        public JugadorController(ILogicaJugador jugadorLogica)
        {
            _jugadorLogica = jugadorLogica;
        }

        [HttpGet]
        public IActionResult IniciarSesion() {
            return View();
        }

        [HttpGet]
        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(RegistroViewModel model)
        {
            try
            {
                bool existe = await _jugadorLogica.ExisteJugador(model.ContrasenaHash);
                if (!existe)
                {
                    TempData["Error"] = "Esa Wallet ya está registrada. Intenta iniciar sesión.";
                    return View(model);
                }

                await _jugadorLogica.RegistrarJugador(
                    model.ContrasenaHash,
                    model.Nombre,
                    model.Apellido,
                    model.Correo
                );

                HttpContext.Session.SetString("UserName", model.Nombre);
                HttpContext.Session.SetString("UserWalletAddress", model.ContrasenaHash);
                TempData["Mensaje"] = "¡Registro exitoso!";

                return RedirectToAction("Index", "Inicio");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IniciarSesion(InicioSesionViewModel model)
        {
            var jugador = await _jugadorLogica.ObtenerJugador(model.ContrasenaHash, model.Correo);
            if (jugador == null)
            {
                TempData["Error"] = "Wallet o Correo incorrecto. Verifica tus datos.";
                return View(model);
            }

            HttpContext.Session.SetString("UserName", jugador.Nombre);
            HttpContext.Session.SetString("UserWalletAddress", jugador.ContrasenaHash);
            return RedirectToAction("Index", "Inicio");
        }

    }
}
