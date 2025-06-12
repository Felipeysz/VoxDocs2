using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VoxDocs.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmpresasContratantes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmpresaContratante = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpresasContratantes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PagamentosConcluidos",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EmpresaContratantePlano = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MetodoPagamento = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataPagamento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataExpiracao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StatusEmpresa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NomePlano = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Periodicidade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPagamentoConcluido = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagamentosConcluidos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PastaPrincipal",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NomePastaPrincipal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmpresaContratante = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PastaPrincipal", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlanosVoxDocs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descriçao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Preco = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Duracao = table.Column<int>(type: "int", nullable: false),
                    Periodicidade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArmazenamentoDisponivel = table.Column<int>(type: "int", nullable: false),
                    LimiteAdmin = table.Column<int>(type: "int", nullable: false),
                    LimiteUsuario = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanosVoxDocs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Usuario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Senha = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PermissionAccount = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmpresaContratante = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlanoPago = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LimiteUsuario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LimiteAdmin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordResetToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordResetTokenExpiration = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubPastas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NomeSubPasta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NomePastaPrincipal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmpresaContratante = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PastaPrincipalModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubPastas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubPastas_PastaPrincipal_PastaPrincipalModelId",
                        column: x => x.PastaPrincipalModelId,
                        principalTable: "PastaPrincipal",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Documentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NomeArquivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UrlArquivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsuarioCriador = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioUltimaAlteracao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataUltimaAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Empresa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NomePastaPrincipal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NomeSubPasta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TamanhoArquivo = table.Column<long>(type: "bigint", nullable: false),
                    NivelSeguranca = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContadorAcessos = table.Column<int>(type: "int", nullable: false),
                    TokenSeguranca = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Descrição = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubPastaModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documentos_SubPastas_SubPastaModelId",
                        column: x => x.SubPastaModelId,
                        principalTable: "SubPastas",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "PlanosVoxDocs",
                columns: new[] { "Id", "ArmazenamentoDisponivel", "Descriçao", "Duracao", "LimiteAdmin", "LimiteUsuario", "Nome", "Periodicidade", "Preco" },
                values: new object[,]
                {
                    { new Guid("0e8c6b83-49c1-403e-b70c-6fc8e0f09c7f"), 200, "Plano completo com desconto de 20% para 12 meses", 12, -1, -1, "Premium", "Anual", 287.90m },
                    { new Guid("7d7f02e7-44b5-4692-88a4-8c2b149b5059"), 200, "Plano completo com desconto de 10% para 6 meses", 6, -1, -1, "Premium", "Semestral", 161.95m },
                    { new Guid("b40c1b56-6cc2-4988-b979-3b00c1dd8e1e"), 200, "Plano completo com funcionalidades avançadas", 1, -1, -1, "Premium", "Mensal", 29.99m },
                    { new Guid("f9b2f7e0-d938-4b4d-b256-38bbd6a9d4ef"), 10, "Plano com funcionalidades básicas", 0, 2, 5, "Gratuito", "Ilimitada", 0m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_SubPastaModelId",
                table: "Documentos",
                column: "SubPastaModelId");

            migrationBuilder.CreateIndex(
                name: "IX_SubPastas_PastaPrincipalModelId",
                table: "SubPastas",
                column: "PastaPrincipalModelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Documentos");

            migrationBuilder.DropTable(
                name: "EmpresasContratantes");

            migrationBuilder.DropTable(
                name: "PagamentosConcluidos");

            migrationBuilder.DropTable(
                name: "PlanosVoxDocs");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "SubPastas");

            migrationBuilder.DropTable(
                name: "PastaPrincipal");
        }
    }
}
