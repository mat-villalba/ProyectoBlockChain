using Nethereum.ABI.FunctionEncoding.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoBlockChain.Logica.Core
{
    [FunctionOutput]
    public class VotoSolidityDTO : IFunctionOutputDTO
    {
        // para leer votos desde blockchain

        [Parameter("address", "jugador", 1)]
        public string Jugador { get; set; }

        [Parameter("uint256", "capituloId", 2)]
        public BigInteger CapituloId { get; set; }

        [Parameter("string", "opcionElegida", 3)]
        public string OpcionElegida { get; set; }

        [Parameter("uint256", "timestamp", 4)]
        public BigInteger Timestamp { get; set; }

        // para logica en backend
        public int PartidaId { get; set; }
    }
}