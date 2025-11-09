using Microsoft.Extensions.Configuration;
using Nethereum.Contracts;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using ProyectoBlockChain.Data.Data;
using ProyectoBlockChain.Logica.Interfaces;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Numerics;

namespace ProyectoBlockChain.Logica
{
    public class JuegoLogica : ILogicaDeJuego
    {
        private readonly AventuraBlockchainDbContext _context;
        private readonly IWeb3 _web3;
        private readonly Contract _contrato;
        private readonly string _cuentaBackendAddress;
        private readonly Account _cuentaBackend;
        private readonly EstadoGlobal _estadoGlobal;

        public JuegoLogica(AventuraBlockchainDbContext context, IConfiguration config, IWeb3 web3, Account cuentaBackend, EstadoGlobal estadoGlobal)
        {
            _context = context;
            _web3 = web3;
            _cuentaBackend = cuentaBackend;
            _estadoGlobal = estadoGlobal;
            string contractAddress = config["BlockchainSettings:ContractAddress"];
            string abi = config["BlockchainSettings:ContractAbi"];
            _contrato = _web3.Eth.GetContract(abi, contractAddress);
        }

        public async Task<Partidum> IniciarNuevaAventura(string idJugadorHost)
        {
            Capitulo primerCapitulo = await obtenerPrimerCapitulo();
            BigInteger idPartidaBlockchain = await registrarPartidaEnBlockChain();

            Partidum nuevaPartida = new Partidum
            {
                Titulo = $"Aventura #{idPartidaBlockchain}",
                FechaCreacion = DateTime.UtcNow,
                HashPrimerBloque = idPartidaBlockchain.ToString(),
                PuntoActualId = primerCapitulo.Id
            };

            var jugadorHost = await _context.Jugadors.FindAsync(idJugadorHost);
            if (jugadorHost == null) throw new Exception("Jugador host no encontrado.");

            nuevaPartida.IdJugadors.Add(jugadorHost);

            _context.Partida.Add(nuevaPartida);
            await _context.SaveChangesAsync();

            _estadoGlobal.ObtenerOIniciarPartida(nuevaPartida.Id, primerCapitulo.Id);

            return nuevaPartida;
        }

        private async Task<BigInteger> registrarPartidaEnBlockChain()
        {
            var funcionIniciar = _contrato.GetFunction("iniciarNuevaPartida");
            var receipt = await funcionIniciar.SendTransactionAndWaitForReceiptAsync(_cuentaBackend.Address);

            var funcionId = _contrato.GetFunction("proximaPartidaId");
            var idPartidaBlockchain = (await funcionId.CallAsync<BigInteger>()) - 1;
            return idPartidaBlockchain;
        }

        private async Task<Capitulo> obtenerPrimerCapitulo()
        {
            Random rand = new Random();
            var capitulosIniciales = await _context.Capitulos
                            .Where(c => c.EsInicio == true)
                            .ToListAsync();
            Capitulo primerCapitulo = capitulosIniciales[rand.Next(capitulosIniciales.Count)];
            return primerCapitulo;
        }

        public async Task UnirseAPartida(string idJugador, int partidaId)
        {
            // Buscar la Partida y asegurarse de cargar su lista de jugadores
            var partida = await _context.Partida
                                .Include(p => p.IdJugadors)
                                .FirstOrDefaultAsync(p => p.Id == partidaId);

            if (partida == null) throw new Exception("Partida no encontrada.");

            // 2. Verificar si el jugador ya está en la lista 
            bool existe = partida.IdJugadors.Any(j => j.Id == idJugador);

            if (!existe)
            {
                // Buscar al jugador que se quiere unir
                var jugadorAUnir = await _context.Jugadors.FindAsync(idJugador);
                if (jugadorAUnir == null) throw new Exception("Jugador no encontrado.");

                partida.IdJugadors.Add(jugadorAUnir);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Capitulo> ObtenerCapituloActual(int partidaId)
        {
            var partida = await _context.Partida.FindAsync(partidaId);
            if (partida == null) throw new Exception("Partida no encontrada.");

            // Inicia el estado en RAM (si no estaba)
            var estadoPartida = _estadoGlobal.ObtenerOIniciarPartida(partida.Id, partida.PuntoActualId);

            var capitulo = await _context.Capitulos.FindAsync(estadoPartida.CapituloActualId);
            if (capitulo == null) throw new Exception("Capítulo actual no encontrado.");

            if (estadoPartida.RondaActiva && (DateTime.UtcNow - estadoPartida.InicioRonda).TotalSeconds > capitulo.TiempoLimiteSegundos)
            {
                return await FinalizarRonda(partidaId);
            }

            return capitulo;
        }

        public async Task Votar(int partidaId, string walletJugador, string opcionElegida)
        {
            // verificar que la ronda esté activa en memoria (el estado global)
            var estadoPartida = _estadoGlobal.ObtenerEstadoPartida(partidaId);
            if (!estadoPartida.RondaActiva) throw new Exception("La votación ha cerrado.");

            // obtiene el capítulo actual de la partida en juego
            var capituloActual = await _context.Capitulos.FindAsync(estadoPartida.CapituloActualId);

            if ((DateTime.UtcNow - estadoPartida.InicioRonda).TotalSeconds > capituloActual.TiempoLimiteSegundos)
            {
                estadoPartida.RondaActiva = false;
                throw new Exception("El tiempo para votar ha expirado.");
            }

            if (opcionElegida != "Opcion1" && opcionElegida != "Opcion2")
            {
                throw new Exception("Voto inválido.");
            }

            // escribe el voto en la DB 
            var nuevoVoto = new Voto
            {
                IdPartida = partidaId,
                IdCapitulo = estadoPartida.CapituloActualId,
                IdJugador = walletJugador,
                OpcionElegida = opcionElegida,
                Timestamp = DateTime.UtcNow
            };
            _context.Votos.Add(nuevoVoto);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw new Exception("Este jugador ya ha votado en esta ronda.");
            }
        }

        public async Task<Capitulo> FinalizarRonda(int partidaId)
        {
            var estadoPartida = _estadoGlobal.ObtenerEstadoPartida(partidaId);
            estadoPartida.RondaActiva = false;// ya termino

            var capituloActual = await _context.Capitulos.FindAsync(estadoPartida.CapituloActualId);

            var partidaDB = await _context.Partida.FindAsync(partidaId);
            if (partidaDB == null) throw new Exception("Partida no encontrada en DB");

            if (capituloActual.EsFinal == true)
            {
                // Si es el final, limpiamos la partida de la memoria
                _estadoGlobal.EliminarPartidaDeMemoria(partidaId);
                await _context.SaveChangesAsync();
                // deberia enviar un evento de "partida finalizada"
                return capituloActual;
            }

            // contar votos 
            var conteo = await ObtenerConteoVotos(partidaId, capituloActual.Id);
            int votosA = conteo["Opcion1"];
            int votosB = conteo["Opcion2"];

            string opcionGanadora;
            bool desempate = false;
            int? proximoCapituloId;

            // logica de Desempate
            desempate = desempatar(capituloActual, votosA, votosB, out opcionGanadora, desempate, out proximoCapituloId);

            // registrar decision en Blockchain 
            var idPartidaBlockchain = BigInteger.Parse(partidaDB.HashPrimerBloque);

            var funcionRegistrar = _contrato.GetFunction("registrarDecision");
            try
            {
                await funcionRegistrar.SendTransactionAndWaitForReceiptAsync(
                    _cuentaBackend.Address, null, null, null,
                    idPartidaBlockchain,
                    (BigInteger)capituloActual.Id,
                    opcionGanadora,
                    (BigInteger)votosA,
                    (BigInteger)votosB,
                    desempate
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al sellar en Blockchain: {ex.Message}");
                estadoPartida.RondaActiva = true; // reactivar si falló
                return capituloActual;
            }

            // una vez registrado --> avanzar al Siguiente Capítulo 
            int proximoIdReal = capituloActual.Id;
            estadoPartida.AvanzarCapitulo(proximoIdReal);

            // Actualizar la DB con el nuevo estado
            partidaDB.PuntoActualId = proximoIdReal;
            await _context.SaveChangesAsync();

            return await _context.Capitulos.FindAsync(proximoIdReal);
        }

        public async Task<Dictionary<string, int>> ObtenerConteoVotos(int partidaId, int capituloId)
        {
            var votosA = await _context.Votos.CountAsync(v =>
                v.IdPartida == partidaId &&
                v.IdCapitulo == capituloId &&
                v.OpcionElegida == "Opcion1");

            var votosB = await _context.Votos.CountAsync(v =>
                v.IdPartida == partidaId &&
                v.IdCapitulo == capituloId &&
                v.OpcionElegida == "Opcion2");

            return new Dictionary<string, int> { { "Opcion1", votosA }, { "Opcion2", votosB } };
        }
        private static bool desempatar(Capitulo capituloActual, int votosA, int votosB, out string opcionGanadora, bool desempate, out int? proximoCapituloId)
        {
            if (votosA > votosB)
            {
                opcionGanadora = "Opcion1";
                proximoCapituloId = capituloActual.IdOpcion1;
            }
            else if (votosB > votosA)
            {
                opcionGanadora = "Opcion2";
                proximoCapituloId = capituloActual.IdOpcion2;
            }
            else
            {
                desempate = true;
                if (new Random().Next(0, 2) == 0)
                {
                    opcionGanadora = "Opcion1";
                    proximoCapituloId = capituloActual.IdOpcion1;
                }
                else
                {
                    opcionGanadora = "Opcion2";
                    proximoCapituloId = capituloActual.IdOpcion2;
                }
            }

            return desempate;
        }

    }
}