using Microsoft.AspNetCore.Mvc;
using ProyectoBlockChain.Logica.Interfaces;
using ProyectoBlockChain.Web.Models;
using Microsoft.AspNetCore.Http;

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
        public async Task<IActionResult> IniciarSesion(InicioSesionViewModel inicioSesion)
        {
            var jugador = await _jugadorLogica.ObtenerJugador(inicioSesion.ContrasenaHash, inicioSesion.Correo);
            if (jugador == null)
            {
                TempData["Error"] = "Wallet o Correo incorrecto. Verifica tus datos.";
                return View(inicioSesion);
            }

            HttpContext.Session.SetString("UserName", jugador.Nombre);
            return RedirectToAction("Index", "Inicio");
        }

    }
}
