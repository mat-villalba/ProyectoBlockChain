using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Numerics;

namespace ProyectoBlockChain.Logica.Core
{   
    [FunctionOutput]
    public class DecisionDTO 
    {
        // sirve para LEER decisiones ya registradas en el contrato

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

        // para logica en backend
        public int PartidaId { get; set; }
        public string TxHash { get; set; } // <-- Agregar esta propiedad para corregir CS1061
    }
}