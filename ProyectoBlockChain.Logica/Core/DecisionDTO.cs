using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Numerics;

namespace ProyectoBlockChain.Logica.Core
{   // TPFInal-PWIII.Logica/Core/DecisionDTO.cs
    /// <summary>
    /// DTO (Data Transfer Object) que actúa como "molde" para que Nethereum 
    /// pueda traducir (deserializar) la 'struct Decision' que devuelve 
    /// el Smart Contract de Solidity (desde la función 'obtenerHistorialPartida').
    /// </summary>
    // [FunctionOutput] le dice a Nethereum que esta clase mapea un resultado
    [FunctionOutput]
    public class DecisionDTO // accesible desde el JuegoLogica
    {
        // Los atributos [Parameter] deben coincidir EXACTAMENTE 
        // con el orden y tipo del 'struct Decision' en Solidity.

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