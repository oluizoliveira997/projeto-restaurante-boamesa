// Boamesa.Infrastructure/Seed/BoamesaContextSeed.cs
using Boamesa.Domain;
using Boamesa.Domain.Entities;
using Boamesa.Domain.Enums;

namespace Boamesa.Infrastructure;

public static class BoamesaContextSeed
{
    public static async Task EnsureSeededAsync(BoamesaContext db)
    {
        if (!db.Mesas.Any())
        {
            db.Mesas.AddRange(new Mesa { Numero = 1, Capacidade = 4 }, new Mesa { Numero = 2, Capacidade = 4 });
        }
        if (!db.ItensCardapio.Any())
        {
            db.ItensCardapio.AddRange(
                new ItemCardapio { Nome = "Prato Almoço 1", Descricao = "…", Periodo = Periodo.Almoco, PrecoBase = 25, Ativo = true },
                new ItemCardapio { Nome = "Prato Jantar 1",  Descricao = "…", Periodo = Periodo.Jantar, PrecoBase = 35, Ativo = true }
            );
        }
        if (!db.Usuarios.Any())
        {
            db.Usuarios.Add(new Usuario { Email = "demo@demo.com", SenhaHash = "x", CriadoEm = DateTime.UtcNow });
        }
        await db.SaveChangesAsync();
    }
}