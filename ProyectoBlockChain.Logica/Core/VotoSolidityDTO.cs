using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoBlockChain.Logica.Core
{
    public class VotoSolidityDTO
    {
        public string jugador { get; set; }
        public uint capituloId { get; set; }
        public string opcionElegida { get; set; }
        public uint timestamp { get; set; }
    }
}
