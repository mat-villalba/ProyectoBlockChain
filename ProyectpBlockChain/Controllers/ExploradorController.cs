using Microsoft.AspNetCore.Mvc;
using Nethereum.ABI.Model;
using Nethereum.Web3;
using ProyectoBlockChain.Logica.Core;
using ProyectoBlockChain.Logica.Interfaces;

namespace ProyectoBlockChain.Web.Controllers
{
    public class ExploradorController : Controller
    {
        private readonly BlockchainSettings _blockchainSettings;
        private readonly ILogicaExplorador _logicaExplorador;

        public ExploradorController(ILogicaExplorador LogicaExplorador, BlockchainSettings blockchainOptions)
        {
            _logicaExplorador = LogicaExplorador;
            _blockchainSettings = blockchainOptions;
        }
        public async Task<IActionResult> VerVotos()
        {
            var votos = await _logicaExplorador.ObtenerTodosLosVotos(
                _blockchainSettings.NodeUrl,
                _blockchainSettings.ContractAddress,
                _blockchainSettings.ContractAbi
                );

            return View(votos);
        }
        public async Task<IActionResult> VerPartidas()
        {
            var partidas = await _logicaExplorador.ObtenerDecisionesFinales(
                _blockchainSettings.NodeUrl,
                _blockchainSettings.ContractAddress,
                _blockchainSettings.ContractAbi
                );

            return View(partidas);
        }
    }
}