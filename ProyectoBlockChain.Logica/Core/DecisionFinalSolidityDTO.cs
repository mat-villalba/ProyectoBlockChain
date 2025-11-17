using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace ProyectoBlockChain.Logica.Core
{
    public class DecisionFinalSolidityDTO
    {
        [Parameter("uint256", "capituloId", 1)]
        public BigInteger CapituloId { get; set; }

        [Parameter("string", "opcionGanadora", 2)]
        public string OpcionGanadora { get; set; }

        [Parameter("uint256", "votosOpcionA", 3)]
        public BigInteger VotosOpcionA { get; set; }

        [Parameter("uint256", "votosOpcionB", 4)]
        public BigInteger VotosOpcionB { get; set; }

        [Parameter("bool", "desempateAplicado", 5)]
        public bool DesempateAplicado { get; set; }

        [Parameter("uint256", "timestamp", 6)]
        public BigInteger Timestamp { get; set; }

    }
}
