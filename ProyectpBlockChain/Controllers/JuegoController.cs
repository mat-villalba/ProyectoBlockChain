using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ProyectoBlockChain.Data.Data;
using ProyectoBlockChain.Logica.Interfaces;
using System.Text.Json;

namespace ProyectoBlockChain.Web.Controllers
{
    public class JuegoController : Controller
    {
        private readonly BlockchainSettings _blockchainSettings;
        private readonly ILogicaDeJuego _logicaJuego;

        public JuegoController(ILogicaDeJuego LogicaDeJuego, BlockchainSettings blockchainOptions)
        {
            _logicaJuego = LogicaDeJuego;
            _blockchainSettings = blockchainOptions;
        }

        public async Task<IActionResult> Jugar()
        {
            Console.WriteLine($"PrivateKey: {_blockchainSettings.BackendPrivateKey}");

            var partidaId = await _logicaJuego.IniciarNuevaPartida(
            _blockchainSettings.NodeUrl,
            _blockchainSettings.BackendPrivateKey,
            _blockchainSettings.ContractAddress,
            _blockchainSettings.ContractAbi
            );

            ViewBag.PartidaId = partidaId;
            ViewBag.CapituloId = 1;
            ViewBag.ContractAddress = _blockchainSettings.ContractAddress;

            return View();
        }

        [HttpGet]
        public IActionResult ObtenerEstadoRonda(int partidaId)
        {
            var estado = _logicaJuego.ObtenerEstadoRonda(partidaId);
            return Json(estado);
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
