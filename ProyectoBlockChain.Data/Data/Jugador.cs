using System;
using System.Collections.Generic;

namespace ProyectoBlockChain.Data.Data;

public partial class Jugador
{
    public string Id { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string ContrasenaHash { get; set; } = null!;

    public DateTime FechaRegistro { get; set; }

    public virtual ICollection<Voto> Votos { get; set; } = new List<Voto>();

    public virtual ICollection<Partidum> IdPartida { get; set; } = new List<Partidum>();
}
