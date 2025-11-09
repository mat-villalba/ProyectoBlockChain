using System;
using System.Collections.Generic;

namespace ProyectoBlockChain.Data.Data;

public partial class Capitulo
{
    public int Id { get; set; }

    public string Descripcion { get; set; } = null!;

    public string Opcion1 { get; set; } = null!;

    public string Opcion2 { get; set; } = null!;

    public int? IdOpcion1 { get; set; }

    public int? IdOpcion2 { get; set; }

    public int TiempoLimiteSegundos { get; set; }

    public bool EsFinal { get; set; }

    public bool EsInicio { get; set; }

    public virtual Capitulo? IdOpcion1Navigation { get; set; }

    public virtual Capitulo? IdOpcion2Navigation { get; set; }

    public virtual ICollection<Capitulo> InverseIdOpcion1Navigation { get; set; } = new List<Capitulo>();

    public virtual ICollection<Capitulo> InverseIdOpcion2Navigation { get; set; } = new List<Capitulo>();

    public virtual ICollection<Partidum> Partida { get; set; } = new List<Partidum>();

    public virtual ICollection<Voto> Votos { get; set; } = new List<Voto>();
}
