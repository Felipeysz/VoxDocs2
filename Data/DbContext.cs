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
        public DbSet<PagamentoCartaoFalsoModel> PagamentosCartao { get; set; }
        public DbSet<PagamentoPixModel> PagamentosPix { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Seed predefined plans
            modelBuilder.Entity<PlanosVoxDocsModel>().HasData(
                // Planos Mensais
                new PlanosVoxDocsModel { Id = 1, Name = "Plano Básico Mensal", Description = "Arquivamento de Dados Com Segurança", Price = 9.99m, Duration = 1, Periodicidade = "Mensal", ArmazenamentoDisponivel = 50, TokensDisponiveis = "Infinito", LimiteAdmin = 1, LimiteUsuario = 5 },
                new PlanosVoxDocsModel { Id = 2, Name = "Plano Intermediário Mensal", Description = "Sistema de Criptografia de Ponta e Comando de Voz", Price = 19.99m, Duration = 1, Periodicidade = "Mensal", ArmazenamentoDisponivel = 50,  TokensDisponiveis = "Infinito", LimiteAdmin = 2, LimiteUsuario = 10 },
                new PlanosVoxDocsModel { Id = 3, Name = "Plano Avançado Mensal", Description = "Todas Funcionalidades", Price = 29.99m, Duration = 1, Periodicidade = "Mensal", ArmazenamentoDisponivel = 50, TokensDisponiveis = "Infinito", LimiteAdmin = 5, LimiteUsuario = 20 },

                // Planos Trimestrais
                new PlanosVoxDocsModel { Id = 4, Name = "Plano Básico Trimestral", Description = "Arquivamento de Dados Com Segurança", Price = 27.99m, Duration = 3, Periodicidade = "Trimestral", ArmazenamentoDisponivel = 100, TokensDisponiveis = "Infinito", LimiteAdmin = 1, LimiteUsuario = 5 },
                new PlanosVoxDocsModel { Id = 5, Name = "Plano Intermediário Trimestral", Description = "Sistema de Criptografia de Ponta e Comando de Voz", Price = 54.99m, Duration = 3, Periodicidade = "Trimestral", ArmazenamentoDisponivel = 100, TokensDisponiveis = "Infinito", LimiteAdmin = 2, LimiteUsuario = 10 },
                new PlanosVoxDocsModel { Id = 6, Name = "Plano Avançado Trimestral", Description = "Todas Funcionalidades", Price = 79.99m, Duration = 3, Periodicidade = "Trimestral", ArmazenamentoDisponivel = 100, TokensDisponiveis = "Infinito", LimiteAdmin = 5, LimiteUsuario = 20 },

                // Planos Semestrais
                new PlanosVoxDocsModel { Id = 7, Name = "Plano Básico Semestral", Description = "Arquivamento de Dados Com Segurança", Price = 49.99m, Duration = 6, Periodicidade = "Semestral", ArmazenamentoDisponivel = 150, TokensDisponiveis = "Infinito", LimiteAdmin = 1, LimiteUsuario = 5 },
                new PlanosVoxDocsModel { Id = 8, Name = "Plano Intermediário Semestral", Description = "Sistema de Criptografia de Ponta e Comando de Voz", Price = 99.99m, Duration = 6, Periodicidade = "Semestral", ArmazenamentoDisponivel = 150, TokensDisponiveis = "Infinito", LimiteAdmin = 2, LimiteUsuario = 10 },
                new PlanosVoxDocsModel { Id = 9, Name = "Plano Avançado Semestral", Description = "Todas Funcionalidades", Price = 149.99m, Duration = 6, Periodicidade = "Semestral", ArmazenamentoDisponivel = 150, LimiteAdmin = 5, TokensDisponiveis = "Infinito", LimiteUsuario = 20 }
            );
        }
    }
}