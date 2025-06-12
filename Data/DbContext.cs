using Microsoft.EntityFrameworkCore;
using VoxDocs.Models;

namespace VoxDocs.Data
{
    public class VoxDocsContext : DbContext
    {
        public VoxDocsContext(DbContextOptions<VoxDocsContext> options)
            : base(options)
        {
        }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<PastaPrincipalModel> PastaPrincipal { get; set; }
        public DbSet<SubPastaModel> SubPastas { get; set; }
        public DbSet<DocumentoModel> Documentos { get; set; }
        public DbSet<PlanosVoxDocsModel> PlanosVoxDocs { get; set; }
        public DbSet<EmpresasContratanteModel> EmpresasContratantes { get; set; }
        public DbSet<PagamentoConcluido> PagamentosConcluidos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed predefined plans
            modelBuilder.Entity<PlanosVoxDocsModel>().HasData(
            // Plano Gratuito
            new PlanosVoxDocsModel
            {
                Id = Guid.Parse("f9b2f7e0-d938-4b4d-b256-38bbd6a9d4ef"),
                Nome = "Gratuito",
                Descriçao = "Plano com funcionalidades básicas",
                Preco = 0m,
                Duracao = 0,
                Periodicidade = "Ilimitada",
                ArmazenamentoDisponivel = 10,
                LimiteAdmin = 2,
                LimiteUsuario = 5
            },

            // Plano Premium base (mensal)
            new PlanosVoxDocsModel
            {
                Id = Guid.Parse("b40c1b56-6cc2-4988-b979-3b00c1dd8e1e"),
                Nome = "Premium",
                Descriçao = "Plano completo com funcionalidades avançadas",
                Preco = 29.99m,
                Duracao = 1,
                Periodicidade = "Mensal",
                ArmazenamentoDisponivel = 200,
                LimiteAdmin = -1,
                LimiteUsuario = -1
            },

            // Plano Premium Semestral
            new PlanosVoxDocsModel
            {
                Id = Guid.Parse("7d7f02e7-44b5-4692-88a4-8c2b149b5059"),
                Nome = "Premium",
                Descriçao = "Plano completo com desconto de 10% para 6 meses",
                Preco = Math.Round(29.99m * 6 * 0.9m, 2), // ~161.95
                Duracao = 6,
                Periodicidade = "Semestral",
                ArmazenamentoDisponivel = 200,
                LimiteAdmin = -1,
                LimiteUsuario = -1
            },

            // Plano Premium Anual
            new PlanosVoxDocsModel
            {
                Id = Guid.Parse("0e8c6b83-49c1-403e-b70c-6fc8e0f09c7f"),
                Nome = "Premium",
                Descriçao = "Plano completo com desconto de 20% para 12 meses",
                Preco = Math.Round(29.99m * 12 * 0.8m, 2), // ~287.90
                Duracao = 12,
                Periodicidade = "Anual",
                ArmazenamentoDisponivel = 200,
                LimiteAdmin = -1,
                LimiteUsuario = -1
            }
        );
        }
    }
}
