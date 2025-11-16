using ProyectoBlockChain.Data.Data;
using ProyectoBlockChain.Logica.Interfaces;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace ProyectoBlockChain.Logica
{
    public class UsuarioLogica : ILogicaJugador
    {
        private readonly AventuraBlockchainDbContext _context;

        public UsuarioLogica(AventuraBlockchainDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExisteJugador(string walletAddress)
        {
            // debe devolver true si existe un jugador con esa walletAddress
            string hashedWalletAddress = walletAddressHash(walletAddress);

            bool existe = false;
            Jugador jugadorBuscado = await _context.Jugadors.FindAsync(hashedWalletAddress);
            if (jugadorBuscado != null)
            {
                existe = true;
                return existe;
            }
            return existe;
        }

        public async Task RegistrarJugador(string walletAddress, string nombreUsuario, string apellido, string correo)
        {

            if (await ExisteJugador(walletAddress))
            {
                throw new Exception("Esta wallet ya está registrada.");
            }
            string hashedWalletAddress = walletAddressHash(walletAddress);

            var jugador = new Jugador
            {
                Id = hashedWalletAddress,
                Nombre = nombreUsuario,
                Apellido = apellido,
                Correo = correo,
                ContrasenaHash = hashedWalletAddress,
                FechaRegistro = DateTime.UtcNow
            };

            _context.Jugadors.Add(jugador);
            await _context.SaveChangesAsync();
        }

        public async Task<Jugador> ObtenerJugador(string walletAddress, string correo)
        {

            if (await ExisteJugador(walletAddress))
            {
                // si exite devuelve el jugador
                string hashedWalletAddress = walletAddressHash(walletAddress);
                var jugador = await _context.Jugadors
                    .Where(j => j.ContrasenaHash == hashedWalletAddress && j.Correo == correo)
                    .FirstOrDefaultAsync();
                return jugador;
            }
            return null;
        }

        public string walletAddressHash(string walletAddress)
        {
            // Usamos SHA-256 para generar el hash de la walletAddress
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(walletAddress));

                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString();
            }

        }
    }
}