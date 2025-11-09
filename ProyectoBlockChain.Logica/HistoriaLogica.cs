using Microsoft.Extensions.Configuration;
using Nethereum.Contracts;
using Nethereum.Web3;
using ProyectoBlockChain.Logica.Core;
using ProyectoBlockChain.Logica.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoBlockChain.Logica
{
    public class HistoriaLogica : ILogicaHistorial
    {
        private readonly Contract _contrato;

        public HistoriaLogica()
        {
            // Lee la configuración directamente desde appsettings.json
            var json = System.IO.File.ReadAllText("appsettings.json");
            var doc = System.Text.Json.JsonDocument.Parse(json);
            var blockchainSection = doc.RootElement.GetProperty("BlockchainSettings");

            string nodeUrl = blockchainSection.GetProperty("NodeUrl").GetString();
            string contractAddress = blockchainSection.GetProperty("ContractAddress").GetString();
            string abi = blockchainSection.GetProperty("ContractAbi").GetRawText();

            // Crea instancia de Web3 solo para lectura
            var web3 = new Web3(nodeUrl);

            _contrato = web3.Eth.GetContract(abi, contractAddress);
        }

        public async Task<List<DecisionDTO>> ObtenerHistorialBlockchain(BigInteger partidaId)
        {
            try
            {
                var funcionHistorial = _contrato.GetFunction("obtenerHistorialPartida");
                var historial = await funcionHistorial.CallDeserializingToObjectAsync<List<DecisionDTO>>(partidaId);
                return historial;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al leer de la Blockchain: {ex.Message}");
                return new List<DecisionDTO>();
            }
        }

    }
}