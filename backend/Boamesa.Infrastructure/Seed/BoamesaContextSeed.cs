// Boamesa.Infrastructure/Seed/BoamesaContextSeed.cs
using Boamesa.Domain;
using Boamesa.Domain.Entities;
using Boamesa.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Boamesa.Infrastructure;

public static class BoamesaContextSeed
{
    public static async Task EnsureSeededAsync(BoamesaContext db)
    {
        // Mesas
        if (!await db.Mesas.AnyAsync())
        {
            db.Mesas.AddRange(
                new Mesa { Numero = 1, Capacidade = 4 },
                new Mesa { Numero = 2, Capacidade = 4 },
                new Mesa { Numero = 3, Capacidade = 2 }
            );
        }

        // Parceiro do app (para DeliveryAplicativo)
        if (!await db.Parceiros.AnyAsync())
        {
            db.Parceiros.Add(new ParceiroApp { Nome = "AppParceiro Demo" });
        }

        // Itens de cardápio com imagem pública (wwwroot/images)
        if (!await db.ItensCardapio.AnyAsync())
        {
            db.ItensCardapio.AddRange(
                new ItemCardapio
                {
                    Nome = "Prato Almoço 1",
                    Descricao = "Prato completo do almoço",
                    Periodo = Periodo.Almoco,
                    PrecoBase = 25m,
                    Ativo = true,
                    ImagemUrl = "/images/almoco1.png" // coloque este arquivo em Boamesa.Api/wwwroot/images
                },
                new ItemCardapio
                {
                    Nome = "Prato Jantar 1",
                    Descricao = "Prato completo do jantar",
                    Periodo = Periodo.Jantar,
                    PrecoBase = 35m,
                    Ativo = true,
                    ImagemUrl = "/images/jantar1.png" // pode ser .png/.webp também
                }
            );
        }

        // Usuário demo
        if (!await db.Usuarios.AnyAsync())
        {
            db.Usuarios.Add(new Usuario
            {
                Email = "demo@demo.com",
                // Simples por enquanto: senha "123456" em texto.
                // Se você já está usando hash no login fake, troque por um hash compatível.
                SenhaHash = "123456",
                CriadoEm = DateTime.UtcNow
            });
        }

        await db.SaveChangesAsync();
    }
}