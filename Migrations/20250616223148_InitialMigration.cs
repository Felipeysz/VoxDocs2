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
                name: "ConfiguracaoDocumentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PermitirPDF = table.Column<bool>(type: "bit", nullable: false),
                    PermitirWord = table.Column<bool>(type: "bit", nullable: false),
                    PermitirExcel = table.Column<bool>(type: "bit", nullable: false),
                    PermitirImagens = table.Column<bool>(type: "bit", nullable: false),
                    TamanhoMaximoMB = table.Column<int>(type: "int", nullable: false),
                    DiasArmazenamentoTemporario = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracaoDocumentos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmpresasContratantes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmpresaContratante = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlanoContratado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataContratacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpresasContratantes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LogsAtividades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    usuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Usuario = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TipoAcao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DataHora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Detalhes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogsAtividades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PagamentosConcluidos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NomePlano = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PeriodicidadePlano = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValorPlano = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MetodoPagamento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataPagamento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StatusEmpresa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmpresaContratante = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailContato = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataExpiracao = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                    Periodicidade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Duracao = table.Column<int>(type: "int", nullable: true),
                    ArmazenamentoDisponivel = table.Column<int>(type: "int", nullable: true),
                    LimiteAdmin = table.Column<int>(type: "int", nullable: true),
                    LimiteUsuario = table.Column<int>(type: "int", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
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
                    PasswordResetTokenExpiration = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UltimoLogin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserStorage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsoArmazenamento = table.Column<int>(type: "int", nullable: false),
                    LimiteArmazenamento = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStorage", x => x.Id);
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
                    UrlArquivo = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                columns: new[] { "Id", "ArmazenamentoDisponivel", "Ativo", "Descriçao", "Duracao", "LimiteAdmin", "LimiteUsuario", "Nome", "Periodicidade", "Preco" },
                values: new object[,]
                {
                    { new Guid("0e8c6b83-49c1-403e-b70c-6fc8e0f09c7f"), 200, false, "Plano completo com 10% de desconto (12 meses)", 12, -1, -1, "Premium Anual", "Anual", 1618.92m },
                    { new Guid("7d7f02e7-44b5-4692-88a4-8c2b149b5059"), 200, false, "Plano completo com 10% de desconto (6 meses)", 6, -1, -1, "Premium Semestral", "Semestral", 809.46m },
                    { new Guid("b40c1b56-6cc2-4988-b979-3b00c1dd8e1e"), 200, false, "Plano completo com funcionalidades avançadas", 1, -1, -1, "Premium", "Mensal", 149.90m },
                    { new Guid("f9b2f7e0-d938-4b4d-b256-38bbd6a9d4ef"), 10, false, "Plano com funcionalidades básicas", 0, 2, 5, "Gratuito", "Ilimitado", 0m }
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
                name: "ConfiguracaoDocumentos");

            migrationBuilder.DropTable(
                name: "Documentos");

            migrationBuilder.DropTable(
                name: "EmpresasContratantes");

            migrationBuilder.DropTable(
                name: "LogsAtividades");

            migrationBuilder.DropTable(
                name: "PagamentosConcluidos");

            migrationBuilder.DropTable(
                name: "PlanosVoxDocs");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "UserStorage");

            migrationBuilder.DropTable(
                name: "SubPastas");

            migrationBuilder.DropTable(
                name: "PastaPrincipal");
        }
    }
}
