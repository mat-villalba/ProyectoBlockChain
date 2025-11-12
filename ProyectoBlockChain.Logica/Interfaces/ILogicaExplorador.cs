using ProyectoBlockChain.Logica.Core;

namespace ProyectoBlockChain.Logica.Interfaces
{
    public interface ILogicaExplorador
    {
        Task<List<VotoDTO>> ObtenerTodosLosVotos(string nodeUrl, string contractAddress, string contractAbi);
    }
}