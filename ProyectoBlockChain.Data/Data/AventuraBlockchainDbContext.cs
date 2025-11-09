using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ProyectoBlockChain.Data.Data;

public partial class AventuraBlockchainDbContext : DbContext
{
    public AventuraBlockchainDbContext()
    {
    }

    public AventuraBlockchainDbContext(DbContextOptions<AventuraBlockchainDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Capitulo> Capitulos { get; set; }

    public virtual DbSet<Jugador> Jugadors { get; set; }

    public virtual DbSet<Partidum> Partida { get; set; }

    public virtual DbSet<Voto> Votos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("DESKTOP-L45I1B7\\SQLEXPRESS;Database=AventuraBlockchainDb;Trusted_Connection=True;TrustServerCertificate =True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Capitulo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Capitulo__3214EC2732498C94");

            entity.ToTable("Capitulo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Opcion1)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Opcion2)
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.HasOne(d => d.IdOpcion1Navigation).WithMany(p => p.InverseIdOpcion1Navigation)
                .HasForeignKey(d => d.IdOpcion1)
                .HasConstraintName("FK_Capitulo_Opcion1");

            entity.HasOne(d => d.IdOpcion2Navigation).WithMany(p => p.InverseIdOpcion2Navigation)
                .HasForeignKey(d => d.IdOpcion2)
                .HasConstraintName("FK_Capitulo_Opcion2");
        });

        modelBuilder.Entity<Jugador>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Jugador__3214EC27FD7B343B");

            entity.ToTable("Jugador");

            entity.HasIndex(e => e.Correo, "UQ_Jugador_Correo").IsUnique();

            entity.Property(e => e.Id)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("ID");
            entity.Property(e => e.Apellido)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Correo)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Partidum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Partida__3214EC278C2931E7");

            entity.HasIndex(e => e.HashPrimerBloque, "UQ_Partida_HashPrimerBloque").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.HashPrimerBloque)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.PuntoActualId).HasDefaultValue(1);
            entity.Property(e => e.Titulo)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.PuntoActual).WithMany(p => p.Partida)
                .HasForeignKey(d => d.PuntoActualId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Partida_PuntoActualId");

            entity.HasMany(d => d.IdJugadors).WithMany(p => p.IdPartida)
                .UsingEntity<Dictionary<string, object>>(
                    "PartidaJugador",
                    r => r.HasOne<Jugador>().WithMany()
                        .HasForeignKey("IdJugador")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_PartidaJugador_Jugador"),
                    l => l.HasOne<Partidum>().WithMany()
                        .HasForeignKey("IdPartida")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_PartidaJugador_Partida"),
                    j =>
                    {
                        j.HasKey("IdPartida", "IdJugador");
                        j.ToTable("PartidaJugador");
                        j.IndexerProperty<string>("IdJugador")
                            .HasMaxLength(128)
                            .IsUnicode(false);
                    });
        });

        modelBuilder.Entity<Voto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Voto__3214EC2772CF310D");

            entity.ToTable("Voto");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.IdCapitulo).HasColumnName("ID_Capitulo");
            entity.Property(e => e.IdJugador)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("ID_Jugador");
            entity.Property(e => e.IdPartida).HasColumnName("ID_Partida");
            entity.Property(e => e.OpcionElegida)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Timestamp).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.IdCapituloNavigation).WithMany(p => p.Votos)
                .HasForeignKey(d => d.IdCapitulo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Voto_Capitulo");

            entity.HasOne(d => d.IdJugadorNavigation).WithMany(p => p.Votos)
                .HasForeignKey(d => d.IdJugador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Voto_Jugador");

            entity.HasOne(d => d.IdPartidaNavigation).WithMany(p => p.Votos)
                .HasForeignKey(d => d.IdPartida)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Voto_Partida");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
