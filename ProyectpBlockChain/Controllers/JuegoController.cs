using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Nethereum.Hex.HexTypes;
using Nethereum.Model;
using Nethereum.Web3;
using ProyectoBlockChain.Data.Data;
using ProyectoBlockChain.Logica;
using ProyectoBlockChain.Logica.Core;
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
        private readonly ITemporizador _temporizador;

        public JuegoController(ILogicaDeJuego LogicaDeJuego, BlockchainSettings blockchainOptions, ITemporizador temporizador)
        {
            _logicaJuego = LogicaDeJuego;
            _blockchainSettings = blockchainOptions;
            _temporizador = temporizador;
        }

        public async Task<IActionResult> Jugar()
        {
            // este metodo debe buscar en la base la el capitulo con sus opciones
            InicioPartidaDTO inicioPartida = await _logicaJuego.IniciarNuevaPartida();

            var capitulo = inicioPartida.Capitulo;
            var partidaId = inicioPartida.PartidaId;    

            _temporizador.IniciarTemporizador(
                partidaId,
                capitulo.Id,
                async () => await _logicaJuego.FinalizarVotacion(partidaId, capitulo.Id)
            );
            inicioPartida.ContractAddress = _blockchainSettings.ContractAddress;
            return View(inicioPartida);
        }

        public async Task<IActionResult> FinalizarVotacion(int partidaId, int capituloId)
        {
            var resultado = await _logicaJuego.FinalizarVotacion(partidaId, capituloId);

            return View(resultado);
        }

    }
}