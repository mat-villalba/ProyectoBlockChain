using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ProyectoBlockChain.Data.Data;
using System.Text.Json;

namespace ProyectoBlockChain.Web.Controllers
{
    public class JuegoController : Controller
    {
        private readonly BlockchainSettings _blockchainSettings;
        public JuegoController(IOptions<BlockchainSettings> blockchainOptions)
        {
            _blockchainSettings = blockchainOptions.Value;
        }
        public IActionResult Jugar(int partidaId = 1, int capituloId = 1)
        {
            var abiPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "abi", "Contrato.json");
            var abi = System.IO.File.ReadAllText(abiPath);

            ViewBag.ContractAddress = _blockchainSettings.ContractAddress;
            ViewBag.ContractAbi = abi;
            ViewBag.PartidaId = partidaId;
            ViewBag.CapituloId = capituloId;
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
