using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProyectoBlockChain.Data.Data;

namespace ProyectoBlockChain.Logica
{
    public class EstadoGlobal
    {
        // Almacena el estado de TODAS las partidas activas
        // Key: IdPartida (de la DB SQL)
        // Value: El estado de esa partida
        private readonly ConcurrentDictionary<int, EstadoPartidaActiva> _partidasActivas = new ConcurrentDictionary<int, EstadoPartidaActiva>();

        // Obtiene el estado de una partida (o la crea en memoria si es la primera vez)
        public EstadoPartidaActiva ObtenerOIniciarPartida(int idPartida, int capituloInicialId)
        {
            return _partidasActivas.GetOrAdd(idPartida, (key) => new EstadoPartidaActiva
            {
                IdPartida = idPartida,
                CapituloActualId = capituloInicialId,
                InicioRonda = DateTime.UtcNow
            });
        }

        public EstadoPartidaActiva ObtenerEstadoPartida(int partidaId)
        {
            // busca en memoria si la partida está activa (si esta es porque esta activa)
            if (_partidasActivas.TryGetValue(partidaId, out var estado))
            {
                return estado;
            }
            throw new Exception("La partida no está activa en la memoria del servidor.");
        }

        public void EliminarPartidaDeMemoria(int idPartida)
        {
            _partidasActivas.TryRemove(idPartida, out _);
        }

        /// <summary>
        /// Objeto que guarda el estado de la partida que se esta jugando
        /// </summary>
        public class EstadoPartidaActiva
        {
            public int IdPartida { get; set; }
            public int CapituloActualId { get; set; }
            public DateTime InicioRonda { get; set; }
            public bool RondaActiva { get; set; } = true;

            // pone activo el seguiente capítulo y reinicia el timer
            public void AvanzarCapitulo(int proximoCapituloId)
            {
                CapituloActualId = proximoCapituloId;
                InicioRonda = DateTime.UtcNow;
                RondaActiva = true;
            }
        }
    }
}