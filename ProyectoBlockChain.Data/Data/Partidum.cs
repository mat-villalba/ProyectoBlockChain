using System;
using System.Collections.Generic;

namespace ProyectoBlockChain.Data.Data;

public partial class Partidum
{
    public int Id { get; set; }

    public string Titulo { get; set; } = null!;

    public DateTime FechaCreacion { get; set; }

    public string HashPrimerBloque { get; set; } = null!;

    public int PuntoActualId { get; set; }

    public virtual Capitulo PuntoActual { get; set; } = null!;

    public virtual ICollection<Voto> Votos { get; set; } = new List<Voto>();

    public virtual ICollection<Jugador> IdJugadors { get; set; } = new List<Jugador>();
}
