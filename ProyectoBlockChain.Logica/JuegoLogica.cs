using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using ProyectoBlockChain.Data.Data;
using ProyectoBlockChain.Logica.Core;
using ProyectoBlockChain.Logica.Interfaces;
using System.Numerics;


namespace ProyectoBlockChain.Logica
{
    public class JuegoLogica : ILogicaDeJuego
    {

        private readonly Web3 _web3;
        private readonly Contract _contrato;
        private readonly Account _cuentaBackend;
        private readonly AventuraBlockchainDbContext _context;


        public JuegoLogica(AventuraBlockchainDbContext context, IConfiguration config, Account cuentaBackend, Web3 web3)
        {
            _context = context;
            _web3 = web3;
            _cuentaBackend = cuentaBackend;
            // inicializar contrato blockchain
            var abi = config["BlockchainSettings:ContractAbi"];
            var address = config["BlockchainSettings:ContractAddress"];

            _contrato = _web3.Eth.GetContract(abi, address);
        }


        // Inicia nueva partida y devulve el capitulo con sus opciones
        public async Task<InicioPartidaDTO> IniciarNuevaPartida()
        {
            var random = new Random();

            // encontrar un capitulo aleatorio que sea de inicio y elijo uno al azar
            var capitulos = _context.Capitulos.Where(c => c.EsInicio == true).ToList();
            if (capitulos == null || capitulos.Count == 0)
            {
                throw new Exception("El capitulo es nulo o no hay capitulos");
            }
            else
            {
                var capituloInicio = capitulos[random.Next(capitulos.Count)];
                var fin = DateTime.UtcNow.AddSeconds(capituloInicio.TiempoLimiteSegundos);

                // registrar nueva partida en blockchain
                _web3.TransactionManager.DefaultGas = new HexBigInteger(300000);
                BigInteger partidaId = await registrarPartidaEnBlockChain();

                return new InicioPartidaDTO
                {
                    PartidaId = partidaId,
                    Capitulo = capituloInicio,
                    FinVotacion = fin
                };
            }
        }

        private async Task<BigInteger> registrarPartidaEnBlockChain()
        {
            var funcionIniciar = _contrato.GetFunction("iniciarNuevaPartida");

            // Ejecutar la transacción
            await funcionIniciar.SendTransactionAndWaitForReceiptAsync(
                _cuentaBackend.Address,
                new HexBigInteger(300000),
                null,
                null
            );

            // Leer el nuevo id
            var funcionId = _contrato.GetFunction("proximaPartidaId");
            var idActual = await funcionId.CallAsync<BigInteger>();

            // Como el contrato incrementa antes de devolver:
            // nuevaPartidaId = proximaPartidaId ANTERIOR
            // la variable ahora está incrementada, así que restamos 1
            return idActual - 1;
        }

        public async Task<InicioPartidaDTO> ObtenerSiguienteCapitulo(BigInteger partidaId, BigInteger capituloId)
        {
            // 1. Traer resultado desde blockchain
            var siguienteCapituloId = await ObtenerCapituloGanador(partidaId, capituloId);

            // 2. Buscar capítulo actual en BD
            var siguienteCapitulo = await _context.Capitulos.FindAsync((int)siguienteCapituloId);

            var fin = DateTime.UtcNow.AddSeconds(siguienteCapitulo.TiempoLimiteSegundos);

            return new InicioPartidaDTO
            {
                PartidaId = partidaId,
                Capitulo = siguienteCapitulo,
                FinVotacion = fin
            };
        }

        private async Task<int?> ObtenerCapituloGanador(BigInteger partidaId, BigInteger capituloId)
        {
            var obtenerVotosFunc = _contrato.GetFunction("obtenerVotos");
            var votos = await obtenerVotosFunc.CallAsync<List<VotoSolidityDTO>>(partidaId);

            if (votos == null)
                throw new Exception("No se pudieron obtener los votos.");

            // Filtrar por capítulo actual
            var votosCapitulo = votos.Where(v => (int)v.CapituloId == (int)capituloId).ToList();

            if (!votosCapitulo.Any())
                return null;

            // Agrupar por ID REAL del capítulo elegido
            var grupos = votosCapitulo
                .GroupBy(v => Convert.ToInt32(v.OpcionElegida))
                .Select(g => new { CapituloId = g.Key, Cant = g.Count() })
                .OrderByDescending(g => g.Cant)
                .ToList();

            var top = grupos[0];

            // Empate
            var empatados = grupos.Where(g => g.Cant == top.Cant).ToList();
            if (empatados.Count > 1)
            {
                var rnd = new Random();
                return empatados[rnd.Next(empatados.Count)].CapituloId;
            }

            return top.CapituloId;
        }

        public async Task<ResultadoVotacionDTO> FinalizarVotacion(BigInteger partidaId, BigInteger capituloId)
        {
            // --- 0) Convertir BigInteger a int (para usar con EF) ---
            if (partidaId < 0 || capituloId < 0)
                throw new ArgumentException("IDs no pueden ser negativos.");
            if (partidaId > int.MaxValue || capituloId > int.MaxValue)
                throw new ArgumentOutOfRangeException("IDs demasiado grandes para ser manejados por la BD.");
            int partidaInt = (int)partidaId;
            int capituloInt = (int)capituloId;

            // 1) Obtener votos desde blockchain
            var obtenerVotosFunc = _contrato.GetFunction("obtenerVotos");
            var votos = await obtenerVotosFunc.CallAsync<List<VotoSolidityDTO>>((uint)partidaInt);

            if (votos == null)
                throw new Exception("No se pudieron obtener los votos.");

            // 2) Filtrar votos por capítulo
            var votosCapitulo = votos.Where(v => v.CapituloId == capituloId).ToList();

            if (!votosCapitulo.Any())
            {
                return new ResultadoVotacionDTO
                {
                    PartidaId = partidaId,
                    CapituloId = capituloId,
                    Ganador = null,
                    Desempate = false,
                    TxHash = null,
                    VotosPorOpcion = new Dictionary<string, int>(),
                    Descripcion = null,
                    TextoOpcionGanadora = null
                };
            }

            // 3) Agrupar dinámicamente por la opción elegida
            var grupos = votosCapitulo
                .GroupBy(v => v.OpcionElegida)
                .Select(g => new { Opcion = g.Key, Cant = g.Count() })
                .OrderByDescending(g => g.Cant)
                .ToList();

            // 4) Determinar ganador + desempate si es necesario
            var top = grupos[0];
            string ganador = top.Opcion;
            bool desempate = false;

            // buscar otros con el mismo conteo
            var empatados = grupos.Where(g => g.Cant == top.Cant).ToList();
            if (empatados.Count > 1)
            {
                var rnd = new Random();
                var elegido = empatados[rnd.Next(empatados.Count)];
                ganador = elegido.Opcion;
                desempate = true;
            }

            int votosGanador = grupos.First(g => g.Opcion == ganador).Cant;
            int votosSegundo = grupos.Where(g => g.Opcion != ganador)
                                     .Select(g => g.Cant)
                                     .DefaultIfEmpty(0)
                                     .Max();

            // 5) Registrar decisión final en blockchain
            var registrar = _contrato.GetFunction("registrarDecisionFinal");

            var tx = await registrar.SendTransactionAsync(
                from: _cuentaBackend.Address,
                gas: new HexBigInteger(300000),
                value: null,
                functionInput: new object[]
                {
                    (uint)partidaInt,
                    (uint)capituloInt,
                    ganador,
                    votosGanador,
                    votosSegundo,
                    desempate
                }
            );

            // 6) Obtener datos del capítulo desde la DB usando capituloInt (int)
            var capitulo = _context.Capitulos.FirstOrDefault(c => c.Id == capituloInt);

            string descripcion = null;
            if (capitulo != null && !string.IsNullOrWhiteSpace(capitulo.Descripcion))
            {
                descripcion = capitulo.Descripcion.Length > 80
                    ? capitulo.Descripcion + "..."
                    : capitulo.Descripcion;
            }

            // Texto de la opción ganadora: ganador contiene la clave (por ejemplo el id del capítulo siguiente)
            string textoGanador = null;
            if (capitulo != null)
            {
                // Si las opciones guardan los ids de capítulo siguientes en IdOpcion1 / IdOpcion2
                if (int.TryParse(ganador, out int ganadorId))
                {
                    if (capitulo.IdOpcion1.HasValue && capitulo.IdOpcion1.Value == ganadorId)
                        textoGanador = capitulo.Opcion1;
                    else if (capitulo.IdOpcion2.HasValue && capitulo.IdOpcion2.Value == ganadorId)
                        textoGanador = capitulo.Opcion2;
                }
                else
                {
                    // Si ganador no es id sino texto (ej: "Opcion1"), adaptá según tu formato:
                    if (ganador.Equals("Opcion1", StringComparison.OrdinalIgnoreCase))
                        textoGanador = capitulo.Opcion1;
                    else if (ganador.Equals("Opcion2", StringComparison.OrdinalIgnoreCase))
                        textoGanador = capitulo.Opcion2;
                }
            }

            // 7) Devolver DTO
            return new ResultadoVotacionDTO
            {
                PartidaId = partidaId,
                CapituloId = capituloId,
                Ganador = ganador,
                Desempate = desempate,
                TxHash = tx,
                VotosPorOpcion = grupos.ToDictionary(g => g.Opcion, g => g.Cant),
                Descripcion = descripcion,
                TextoOpcionGanadora = textoGanador
            };
        }
        public async Task<List<CapituloJugadoDTO>> ObtenerHistorialPartida(BigInteger partidaId)
        {
            var historial = new List<CapituloJugadoDTO>();

            // 1. Traer todas las decisiones finales de la partida desde blockchain
            var obtenerDecisionesFunc = _contrato.GetFunction("obtenerHistorialPartida");
            var decisiones = await obtenerDecisionesFunc.CallAsync<List<DecisionFinalSolidityDTO>>(partidaId);

            if (decisiones == null || !decisiones.Any())
                return historial;

            var decisionesPorCapitulo = decisiones
                                        .GroupBy(d => d.CapituloId)
                                        .OrderBy(g => g.Min(d => d.Timestamp)); // orden cronológico

            foreach (var grupo in decisionesPorCapitulo)
            {
                var decision = grupo.First(); 
                var capitulo = await _context.Capitulos.FindAsync((int)decision.CapituloId);

                if (capitulo != null)
                {
                    historial.Add(new CapituloJugadoDTO
                    {
                        CapituloId = capitulo.Id,
                        Descripcion = capitulo.Descripcion,
                        OpcionElegida = decision.OpcionGanadora
                    });
                }
            }
            return historial;
        }
    }
}