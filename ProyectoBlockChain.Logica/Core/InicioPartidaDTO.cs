using ProyectoBlockChain.Data.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoBlockChain.Logica.Core
{
    public class InicioPartidaDTO
    {
        public BigInteger PartidaId { get; set; }
        public Capitulo Capitulo { get; set; }
        public DateTime FinVotacion { get; set; }
        public string ContractAddress { get; set; }
    }
}
