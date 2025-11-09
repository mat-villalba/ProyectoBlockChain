using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ProyectoBlockChain.Logica.Core;

namespace ProyectoBlockChain.Logica.Interfaces
{
    public interface ILogicaHistorial
    {
        Task<List<DecisionDTO>> ObtenerHistorialBlockchain(BigInteger historiaId);
    }
}