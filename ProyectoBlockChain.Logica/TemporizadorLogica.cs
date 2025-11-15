using ProyectoBlockChain.Logica.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Numerics;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace ProyectoBlockChain.Logica
{
    public class TemporizadorLogica : ITemporizador
    {
        private static readonly ConcurrentDictionary<string, Timer> _timers = new();

        // Inicia un temporizador que ejecuta un callback a finalizarVotacion
        // Si ya existe un timer en capítulo, no hace nada
        public void IniciarTemporizador(BigInteger partidaId, BigInteger capituloId, Func<Task> callback, int segundos = 30)
        {
            if (callback == null) throw new ArgumentNullException(nameof(callback));

            string key = $"{partidaId}-{capituloId}";

            // Si ya existe, no duplicamos
            if (_timers.ContainsKey(key)) return;

            var timer = new Timer(segundos * 1000) // valor en ms
            {
                AutoReset = false,
                Enabled = false
            };

            // ejecuta el callback
            timer.Elapsed += async (s, e) =>
            {
                try
                {
                    // Detiene el timer antes de ejecutar 
                    if (_timers.TryRemove(key, out var removed))
                    {
                        try { removed.Stop(); removed.Dispose(); } catch { }
                    }

                    // Ejecutar callback
                    await callback().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error en temporizador {key}: {ex}");
                }
            };

            // arrancar el timer si se agregó correctamente
            if (_timers.TryAdd(key, timer))
            {
                timer.Start();
            }
            else
            {
                try { timer.Dispose(); } catch { }
            }
        }

        public void CancelarTemporizador(BigInteger partidaId, BigInteger capituloId)
        {
            string key = $"{partidaId}-{capituloId}";
            if (_timers.TryRemove(key, out var t))
            {
                try { t.Stop(); t.Dispose(); } catch { }
            }
        }
    }
}
