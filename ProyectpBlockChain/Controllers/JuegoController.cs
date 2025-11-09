using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ProyectoBlockChain.Logica;

namespace ProyectoBlockChain.Web.Controllers
{
    public class JuegoController : Controller
    {
        private JuegoLogica _servicioJuego;

        public JuegoController(JuegoLogica servicioMetodo)
        {
            _servicioJuego = servicioMetodo;
        }

        public IActionResult Jugar()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CrearPartida(string idJugadorHost)
        {
            idJugadorHost ??= "12345OP";

            var nuevaPartida = _servicioJuego.IniciarNuevaAventura(idJugadorHost);
            return View("Sala", nuevaPartida);
        }

        public IActionResult Unirse()
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
