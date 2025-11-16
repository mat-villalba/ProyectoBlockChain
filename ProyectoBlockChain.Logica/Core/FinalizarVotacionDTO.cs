using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace ProyectoBlockChain.Logica.Core
{
    public class FinalizarVotacionDTO
    {
        public BigInteger PartidaId { get; set; }
        public BigInteger CapituloId { get; set; }
    }
}
