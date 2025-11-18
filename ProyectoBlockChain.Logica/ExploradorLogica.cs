using Microsoft.Extensions.Configuration;
using Nethereum.Contracts;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using ProyectoBlockChain.Data.Data;
using ProyectoBlockChain.Logica.Core;
using ProyectoBlockChain.Logica.Interfaces;
using System.Numerics;

namespace ProyectoBlockChain.Logica
{
    public class ExploradorLogica : ILogicaExplorador
    {

        private readonly Web3 _web3;
        private readonly Contract _contrato;
        private readonly Account _cuentaBackend;


        public ExploradorLogica(IConfiguration config, Account cuentaBackend, Web3 web3)
        {
            _web3 = web3;
            _cuentaBackend = cuentaBackend;
            var abi = config["BlockchainSettings:ContractAbi"];
            var address = config["BlockchainSettings:ContractAddress"];

            _contrato = _web3.Eth.GetContract(abi, address);
        }

        public async Task<List<VotoSolidityDTO>> ObtenerTodosLosVotos()
        {
            var funcionProxima = _contrato.GetFunction("proximaPartidaId");
            var totalPartidas = await funcionProxima.CallAsync<BigInteger>();

            var funcionVotos = _contrato.GetFunction("obtenerVotos");

            var votosTotales = new List<VotoSolidityDTO>();

            for (int i = 0; i < (int)totalPartidas; i++)
            {
                var result = await funcionVotos.CallDeserializingToObjectAsync<ObtenerVotosOutputDTO>(i);
                var votos = result.Votos ?? new List<VotoSolidityDTO>();

                foreach (var voto in votos)
                {
                    voto.PartidaId = i;
                }

                votosTotales.AddRange(votos);
            }

            return votosTotales;
        }

    }
}