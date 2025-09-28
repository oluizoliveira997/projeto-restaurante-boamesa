using Microsoft.EntityFrameworkCore;
using Boamesa.Domain;             // <-- adicione de volta
using Boamesa.Domain.Entities;    // <-- mantenha

namespace Boamesa.Infrastructure;

public class BoamesaContext : DbContext
{
    public BoamesaContext(DbContextOptions<BoamesaContext> options) : base(options) {}

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<PerfilCliente> Perfis => Set<PerfilCliente>();
    public DbSet<EnderecoEntrega> Enderecos => Set<EnderecoEntrega>();
    public DbSet<Ingrediente> Ingredientes => Set<Ingrediente>();
    public DbSet<ItemCardapio> ItensCardapio => Set<ItemCardapio>();
    public DbSet<SugestaoDoChefe> Sugestoes => Set<SugestaoDoChefe>();
    public DbSet<Atendimento> Atendimentos => Set<Atendimento>();
    public DbSet<AtendimentoPresencial> AtendPresenciais => Set<AtendimentoPresencial>();
    public DbSet<AtendimentoDeliveryProprio> AtendDeliveryProprio => Set<AtendimentoDeliveryProprio>();
    public DbSet<AtendimentoDeliveryAplicativo> AtendDeliveryApp => Set<AtendimentoDeliveryAplicativo>();
    public DbSet<ParceiroApp> Parceiros => Set<ParceiroApp>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<PedidoItem> PedidoItens => Set<PedidoItem>();
    public DbSet<Mesa> Mesas => Set<Mesa>();
    public DbSet<Reserva> Reservas => Set<Reserva>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        // Usuario 1-1 PerfilCliente
        mb.Entity<Usuario>()
          .HasOne(u => u.PerfilCliente)
          .WithOne(p => p.Usuario)
          .HasForeignKey<PerfilCliente>(p => p.UsuarioId)
          .OnDelete(DeleteBehavior.Cascade);

        // Usuario 1-N Enderecos/Reservas/Pedidos
        mb.Entity<Usuario>()
          .HasMany(u => u.Enderecos)
          .WithOne(e => e.Usuario)
          .HasForeignKey(e => e.UsuarioId);

        mb.Entity<Usuario>()
          .HasMany(u => u.Pedidos)
          .WithOne(p => p.Usuario)
          .HasForeignKey(p => p.UsuarioId);

        mb.Entity<Usuario>()
          .HasMany(u => u.Reservas)
          .WithOne(r => r.Usuario)
          .HasForeignKey(r => r.UsuarioId);

        // TPH para Atendimento (herança)
        mb.Entity<Atendimento>()
          .HasDiscriminator<string>("Tipo")
          .HasValue<AtendimentoPresencial>("Presencial")
          .HasValue<AtendimentoDeliveryProprio>("DeliveryProprio")
          .HasValue<AtendimentoDeliveryAplicativo>("DeliveryAplicativo");

        // N-N ItemCardapio <-> Ingrediente
        mb.Entity<ItemCardapio>()
          .HasMany(i => i.Ingredientes)
          .WithMany(g => g.Itens)
          .UsingEntity(j => j.ToTable("ItemCardapioIngrediente"));

        // Pedido 1-N PedidoItem
        mb.Entity<Pedido>()
          .HasMany(p => p.Itens)
          .WithOne(i => i.Pedido)
          .HasForeignKey(i => i.PedidoId);

        // PedidoItem -> ItemCardapio (N-1)
        mb.Entity<PedidoItem>()
          .HasOne(pi => pi.ItemCardapio)
          .WithMany(ic => ic.PedidoItens)
          .HasForeignKey(pi => pi.ItemCardapioId);

        // SugestaoDoChefe -> ItemCardapio (1-1 por registro)
        mb.Entity<SugestaoDoChefe>()
          .HasOne(s => s.ItemCardapio)
          .WithMany()
          .HasForeignKey(s => s.ItemCardapioId);

        // Mesa 1-N Reserva
        mb.Entity<Mesa>()
          .HasMany(m => m.Reservas)
          .WithOne(r => r.Mesa)
          .HasForeignKey(r => r.MesaId);

        // ---- Precisão de decimais ----
        // Monetários (2 casas)
        mb.Entity<ItemCardapio>()
          .Property(p => p.PrecoBase)
          .HasPrecision(10, 2);

        mb.Entity<PedidoItem>()
          .Property(p => p.PrecoUnitario)
          .HasPrecision(10, 2);

        mb.Entity<PedidoItem>()
          .Property(p => p.DescontoAplicado)
          .HasPrecision(10, 2);

        mb.Entity<AtendimentoDeliveryProprio>()
          .Property(p => p.TaxaFixa)
          .HasPrecision(10, 2);

        mb.Entity<AtendimentoDeliveryAplicativo>()
          .Property(p => p.TaxaFixaParceiro)
          .HasPrecision(10, 2);

        // Percentuais (fração: 0.20 = 20%)
        mb.Entity<AtendimentoDeliveryAplicativo>()
          .Property(p => p.ComissaoPercentual)
          .HasPrecision(5, 4);

        mb.Entity<SugestaoDoChefe>()
          .Property(p => p.DescontoPercentual)
          .HasPrecision(5, 4);

        // Índices úteis
        mb.Entity<SugestaoDoChefe>()
          .HasIndex(s => new { s.Data, s.Periodo })
          .IsUnique(); // garante 1 por dia/periodo
    }
}