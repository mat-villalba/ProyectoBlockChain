using ProyectoBlockChain.Logica.Core;
using System.Numerics;

namespace ProyectoBlockChain.Logica.Interfaces
{
    public interface ILogicaExplorador
    {
        Task<List<VotoSolidityDTO>> ObtenerTodosLosVotos(string nodeUrl, string contractAddress, string contractAbi);
        /*Task<List<DecisionFinalSolidityDTO>> ObtenerDecisionesFinales(string nodeUrl, string contractAddress, string contractAbi, BigInteger idPartida);*/
    }
}