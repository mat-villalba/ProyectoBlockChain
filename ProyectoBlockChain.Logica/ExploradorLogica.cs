using Nethereum.Web3;
using ProyectoBlockChain.Logica.Core;
using ProyectoBlockChain.Logica.Interfaces;
using System.Numerics;

namespace ProyectoBlockChain.Logica
{
    public class ExploradorLogica : ILogicaExplorador
    {

        public async Task<List<VotoSolidityDTO>> ObtenerTodosLosVotos(string nodeUrl, string contractAddress, string contractAbi)
        {
            var web3 = new Web3(nodeUrl);
            var contrato = web3.Eth.GetContract(contractAbi, contractAddress);

            var funcionProxima = contrato.GetFunction("proximaPartidaId");
            var totalPartidas = await funcionProxima.CallAsync<BigInteger>();

            var funcionVotos = contrato.GetFunction("obtenerVotos");

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
       /* public async Task<List<DecisionFinalSolidityDTO>> ObtenerDecisionesFinales(string nodeUrl, string contractAddress, string contractAbi, BigInteger idPartida)
        {
            var web3 = new Web3(nodeUrl);
            var contrato = web3.Eth.GetContract(contractAbi, contractAddress);

            var funcion = contrato.GetFunction("obtenerHistorialPartida");
            var decisiones = await funcion.CallDeserializingToObjectAsync<List<DecisionFinalSolidityDTO>>(idPartida);


            foreach (var d in decisiones)
                d.PartidaId = (int)idPartida;

            return decisiones;
        } */

    }
}