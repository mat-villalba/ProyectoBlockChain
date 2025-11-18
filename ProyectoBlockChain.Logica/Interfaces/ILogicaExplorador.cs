using ProyectoBlockChain.Logica.Core;
using System.Numerics;

namespace ProyectoBlockChain.Logica.Interfaces
{
    public interface ILogicaExplorador
    {
        Task<List<VotoSolidityDTO>> ObtenerTodosLosVotos();
    }
}