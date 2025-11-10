using Microsoft.AspNetCore.Mvc;
using ProyectoBlockChain.Logica;
using ProyectoBlockChain.Logica.Interfaces;

namespace ProyectoBlockChain.Web.Controllers
{
    public class JuegoController : Controller
    {
        private readonly ILogicaDeJuego _logicaDeJuego;
        private readonly ILogicaHistorial _historialLogica;
        private readonly ILogicaJugador _jugadorLogica;

        public JuegoController(ILogicaDeJuego juegoLogica, ILogicaHistorial historialLogica, ILogicaJugador jugadorLogica)
        {
            _logicaDeJuego = juegoLogica;
            _historialLogica = historialLogica;
            _jugadorLogica = jugadorLogica;
        }

        // GET: /Juego/
        public IActionResult Index()
        {
            // Muestra la vista principal (Home/Lobby)
            return View();
        }

        // GET: /Juego/Partida/5
        // (Esta es la vista principal de votación)
        [HttpGet("Juego/Partida/{partidaId}")]
        public async Task<IActionResult> Partida(int partidaId)
        {
            try
            {
                // Sincroniza al jugador y obtiene el capítulo
                var capituloActual = await _logicaDeJuego.ObtenerCapituloActual(partidaId);

                // Obtener el conteo inicial (si no hay polling)
                var conteoVotos = await _logicaDeJuego.ObtenerConteoVotos(partidaId, capituloActual.Id);
                ViewData["VotosA"] = conteoVotos.GetValueOrDefault("Opcion1");
                ViewData["VotosB"] = conteoVotos.GetValueOrDefault("Opcion2");

                // 3. Pasa el capítulo a la vista para que se dibuje
                return View("Votacion", capituloActual);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
