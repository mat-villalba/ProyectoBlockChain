using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoBlockChain.Logica.Interfaces
{
    public interface ITemporizador
    {
        void IniciarTemporizador(BigInteger partidaId, BigInteger capituloId, Func<Task> callback, int segundos = 30);
        void CancelarTemporizador(BigInteger partidaId, BigInteger capituloId);
    }
}
