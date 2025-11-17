using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoBlockChain.Logica.Core
{
    // diseñado para ENVIAR y MOSTRAR el resultado al usuario
    public class ResultadoVotacionDTO
    {
        public BigInteger PartidaId { get; set; }
        public BigInteger CapituloId { get; set; }
        public string Ganador { get; set; }
        public bool Desempate { get; set; }
        public string TxHash { get; set; }
        public Dictionary<string, int> VotosPorOpcion { get; set; }
    }
}
