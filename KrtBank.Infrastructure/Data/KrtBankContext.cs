using KrtBank.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KrtBank.Infrastructure.Data;

public class KrtBankContext : DbContext
{
    public KrtBankContext(DbContextOptions<KrtBankContext> options) : base(options)
    {
    }

    public DbSet<Conta> Contas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Conta>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NomeTitular).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Cpf).HasConversion(
                v => v.Valor,
                v => new Domain.ValueObjects.Cpf(v)
            ).IsRequired().HasMaxLength(11);
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.DataCriacao).IsRequired();
            entity.Property(e => e.DataAtualizacao);
            
            entity.HasIndex(e => e.Cpf).IsUnique();
        });

        base.OnModelCreating(modelBuilder);
    }
}

