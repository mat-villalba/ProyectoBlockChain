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

        /*
        /// <summary>
        /// Almacena el voto de un jugador (off-chain) en la base de datos
        /// para la ronda actual.
        /// </summary>
        Task Votar(int partidaId, string walletJugador, string opcionElegida);

        /// <summary>
        /// Procesa la ronda: cuenta votos, aplica desempate, sella en la blockchain
        /// y avanza al siguiente capítulo.
        /// </summary>
        Task<Capitulo> FinalizarRonda(int partidaId);

        /// <summary>
        /// El jugador inicia una nueva partida --> devuelve la partida creada 
        /// Se redirige al jugador a la pantalla de votacion con el IdPartida.
        /// ahi el controlador usa el id para saber qué capítulo mostrar
        /// </summary>
        Task<Partidum> IniciarNuevaAventura(string idJugadorHost);

        /// <summary>
        /// Un nuevo jugador se une a la partida en curso.
        /// </summary>
        Task UnirseAPartida(string idJugador, int partidaId);

        /// <summary>
        /// recibe el id de la partida y devuelve el capítulo actual.
        /// Controlador usa este método para saber qué capítulo mostrar en la pantalla de votación.
        /// </summary>
        Task<Capitulo> ObtenerCapituloActual(int partidaId);

        /// <summary>
        /// Obtiene el conteo de votos actual de la DB SQL.
        /// Puede ser usado por FinalizarRonda
        /// </summary>
        Task<Dictionary<string, int>> ObtenerConteoVotos(int partidaId, int capituloId);
        */
    }
}