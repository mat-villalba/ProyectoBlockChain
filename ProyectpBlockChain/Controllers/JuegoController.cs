using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ProyectoBlockChain.Data.Data;
using ProyectoBlockChain.Logica.Interfaces;
using System.Numerics;
using System.Text.Json;
using System.Threading.Tasks;

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
        public async Task<IActionResult> FinalizarVotacion(BigInteger idPartida, BigInteger idCapitulo)
        {
            var resultadoVotacion = await _logicaJuego.ResultadoVotacion(
                _blockchainSettings.NodeUrl,
                _blockchainSettings.BackendPrivateKey,
                _blockchainSettings.ContractAddress,
                _blockchainSettings.ContractAbi,
                idPartida, idCapitulo);

            return View(resultadoVotacion);
        }
        public IActionResult FinalizarJuego()
        {
            return View();
        }
    }
}