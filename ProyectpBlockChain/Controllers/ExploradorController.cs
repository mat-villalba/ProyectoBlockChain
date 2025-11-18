using Microsoft.AspNetCore.Mvc;
using Nethereum.ABI.Model;
using Nethereum.Web3;
using ProyectoBlockChain.Logica.Core;
using ProyectoBlockChain.Logica.Interfaces;
using System.Numerics;

namespace ProyectoBlockChain.Web.Controllers
{
    public class ExploradorController : Controller
    {
        private readonly ILogicaExplorador _logicaExplorador;

        public ExploradorController(ILogicaExplorador LogicaExplorador)
        {
            _logicaExplorador = LogicaExplorador;;
        }
        public async Task<IActionResult> VerVotos()
        {
            var votos = await _logicaExplorador.ObtenerTodosLosVotos();

            return View(votos);
        }
    }
}