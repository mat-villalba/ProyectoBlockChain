using System;
using System.Collections.Generic;

namespace ProyectoBlockChain.Data.Data;

public partial class Voto
{
    public Voto() { }
    public Voto(int partidaId, int capituloId, string opcionElegida, string idJugador, DateTime fecha)
    {
        IdPartida = partidaId;
        IdCapitulo = capituloId;
        OpcionElegida = opcionElegida;
        IdJugador = idJugador;
        Timestamp = fecha;
    }
                        
    public int Id { get; set; }

    public string IdJugador { get; set; } = null!;

    public int IdPartida { get; set; }

    public int IdCapitulo { get; set; }

    public string OpcionElegida { get; set; } = null!;

    public DateTime Timestamp { get; set; }

    public virtual Capitulo IdCapituloNavigation { get; set; } = null!;

    public virtual Jugador IdJugadorNavigation { get; set; } = null!;

    public virtual Partidum IdPartidaNavigation { get; set; } = null!;
}
