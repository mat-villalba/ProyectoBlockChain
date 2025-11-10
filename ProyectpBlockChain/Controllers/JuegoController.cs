using Microsoft.AspNetCore.Mvc;
using ProyectoBlockChain.Logica;
using ProyectoBlockChain.Logica.Interfaces;
using ProyectoBlockChain.Web.Models;

namespace ProyectoBlockChain.Web.Controllers
{
    public class JuegoController : Controller
    {
        private readonly ILogicaDeJuego _juegoLogica;
        private readonly ILogicaHistorial _historialLogica;
        private readonly ILogicaJugador _jugadorLogica; 

        public JuegoController(ILogicaDeJuego juegoLogica, ILogicaHistorial historialLogica, ILogicaJugador jugadorLogica)
        {
            _juegoLogica = juegoLogica;
            _historialLogica = historialLogica;
            _jugadorLogica = jugadorLogica;
        }

        // POST: /Juego/CrearPartida
        // (Llamado por el JavaScript de Inicio/Index.cshtml)
        [HttpPost("CrearPartida")]
        public async Task<IActionResult> CrearPartida()
        {
            // Leemos la wallet del Host desde la Sesión 
            var idJugadorHost = HttpContext.Session.GetString("UserWalletAddress");
            if (string.IsNullOrEmpty(idJugadorHost))
            {
                return Unauthorized(new { error = "Usuario no autenticado." });
            }

            try
            {
                // Llama al servicio para crear la Partida en SQL y Blockchain
                var nuevaPartida = await _juegoLogica.IniciarNuevaAventura(idJugadorHost);

                // Devuelve el ID de la partida (de SQL) para que JS pueda redirigir
                return Ok(new { partidaId = nuevaPartida.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        // POST: /Juego/Unirse/5
        [HttpPost("Unirse/{partidaId}")]
        public async Task<IActionResult> Unirse(int partidaId)
        {
            var idJugador = HttpContext.Session.GetString("UserWalletAddress");
            if (string.IsNullOrEmpty(idJugador))
            {
                return Unauthorized(new { error = "Usuario no autenticado." });
            }

            try
            {
                await _juegoLogica.UnirseAPartida(idJugador, partidaId);
                return Ok(new { mensaje = "Unido exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // POST: /Juego/Votar/5
        [HttpPost("Votar/{partidaId}")]
        public async Task<IActionResult> Votar(int partidaId, [FromBody] VotoRequest request)
        {
            var walletJugador = HttpContext.Session.GetString("UserWalletAddress");
            if (string.IsNullOrEmpty(walletJugador))
            {
                return Unauthorized(new { error = "Usuario no autenticado." });
            }

            try
            {
                // Guarda el voto en la DB SQL
                await _juegoLogica.Votar(partidaId, walletJugador, request.OpcionElegida);
                return Ok(new { mensaje = "Voto registrado." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        // POST: /Juego/FinalizarRonda/5
        [HttpPost("FinalizarRonda/{partidaId}")]
        public async Task<IActionResult> FinalizarRonda(int partidaId)
        {
            var walletJugador = HttpContext.Session.GetString("UserWalletAddress");
            if (string.IsNullOrEmpty(walletJugador))
            {
                return Unauthorized(new { error = "Usuario no autenticado." });
            }

            try
            {
                //  Sella en Blockchain
                // Avanza el estado
                // Devuelve el NUEVO capítulo
                var proximoCapitulo = await _juegoLogica.FinalizarRonda(partidaId);
                return Ok(proximoCapitulo);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // GET: /Juego/Partida/5
        // Esta es la vista principal de votación
        [HttpGet("Partida/{partidaId}")]
        public async Task<IActionResult> Partida(int partidaId)
        { 
            var walletJugador = HttpContext.Session.GetString("UserWalletAddress");
            if (string.IsNullOrEmpty(walletJugador))
            {
                TempData["Error"] = "Debes iniciar sesión para ver una partida.";
                return RedirectToAction("IniciarSesion", "Jugador");
            }

            try
            {
              
                // 1. Sincroniza al jugador y obtiene el capítulo
                var capituloActual = await _juegoLogica.ObtenerCapituloActual(partidaId);

                // 2. Pasamos el ID de la partida a la vista (para las llamadas API)
                ViewData["PartidaId"] = partidaId;
                ViewData["WalletJugador"] = walletJugador; // Para el JS de Votar

                // 3. Pasa el capítulo a la vista para que se dibuje
                return View("Votacion", capituloActual); // (La vista debe llamarse Votacion.cshtml)
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Inicio");
            }
        }

        // GET: /Juego/Historial
        [HttpGet("Historial")]
        public async Task<IActionResult> Historial()
        {
            // Esta vista debería mostrar una lista de 'Partidas' de la DB SQL
            // Al hacer clic en una, se mostraría el historial de la Blockchain
            //Esta lógica debe ser implementada
            try
            {
                // Lógica simplificada: muestra el historial de la Partida 1
                var historial = await _historialLogica.ObtenerHistorialBlockchain(1);
                return View("Historial", historial); // La vista debe llamarse Historial.cshtml
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Inicio");
            }
        }
    }
}
