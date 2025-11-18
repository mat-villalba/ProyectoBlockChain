using ProyectoBlockChain.Data.Data;
using ProyectoBlockChain.Logica.Core;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace ProyectoBlockChain.Logica.Interfaces
{
    /// <summary>
    /// Maneja la lógica de la partida
    /// </summary>
    public interface ILogicaDeJuego
    {
        Task<InicioPartidaDTO> IniciarNuevaPartida();
        Task<ResultadoVotacionDTO> FinalizarVotacion(BigInteger partidaId, BigInteger capituloId);
        Task<InicioPartidaDTO> ObtenerSiguienteCapitulo(BigInteger partidaId, BigInteger capituloId);
        Task<List<CapituloJugadoDTO>> ObtenerHistorialPartida(BigInteger partidaId);
    }
}