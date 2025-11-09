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
        private readonly IWeb3 _web3;
        private readonly Contract _contrato;

        public HistoriaLogica(IWeb3 web3, IConfiguration config)
        {
            _web3 = web3;
            string contractAddress = config["BlockchainSettings:ContractAddress"];
            string abi = config["BlockchainSettings:ContractAbi"];
            _contrato = _web3.Eth.GetContract(abi, contractAddress);
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