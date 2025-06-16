// Services/AdminStatisticsService.cs
using VoxDocs.DTO;
using VoxDocs.Models;

namespace VoxDocs.Services
{
    public class AdminStatisticsService : IAdminStatisticsService
    {
        private readonly IAdminStatisticsBusinessRules _businessRules;

        public AdminStatisticsService(IAdminStatisticsBusinessRules businessRules)
        {
            _businessRules = businessRules;
        }

        public async Task<DTOEstatisticasAdmin> GetAdminStatisticsAsync()
        {
            var stats = await _businessRules.ObterEstatisticasAdminAsync();
            
            return new DTOEstatisticasAdmin
            {
                TotalUsuarios = stats.TotalUsuarios,
                UsuariosAtivos = stats.UsuariosAtivos,
                TotalAdministradores = stats.TotalAdministradores,
                UsuariosRecentes = stats.UsuariosRecentes?.Select(ToUsuarioInfoDto).ToList() ?? new List<DTOUsuarioInfo>(),
                TotalEmpresas = stats.TotalEmpresas,
                TotalPlanosAtivos = stats.TotalPlanosAtivos,
                DataAtualizacao = stats.DataAtualizacao
            };
        }

        public async Task<DTOEstatisticasPlano> GetPlanStatisticsAsync(string planoNome)
        {
            var stats = await _businessRules.ObterEstatisticasPlanoAsync(planoNome);
            
            return new DTOEstatisticasPlano
            {
                NomePlano = stats.NomePlano,
                TotalUsuarios = stats.TotalUsuarios,
                UsuariosAtivos = stats.UsuariosAtivos,
                AdministradoresAtivos = stats.AdministradoresAtivos,
                PercentualUso = stats.PercentualUso,
                DataAtualizacao = stats.DataAtualizacao
            };
        }

        public async Task<DTOEstatisticasEmpresa> GetCompanyStatisticsAsync(string empresaNome)
        {
            var stats = await _businessRules.ObterEstatisticasEmpresaAsync(empresaNome);
            
            return new DTOEstatisticasEmpresa
            {
                NomeEmpresa = stats.NomeEmpresa,
                TotalUsuarios = stats.TotalUsuarios,
                UsuariosAtivos = stats.UsuariosAtivos,
                PlanoContratado = stats.PlanoContratado,
                DataContratacao = stats.DataContratacao,
                DataRenovacao = stats.DataRenovacao,
                DataAtualizacao = stats.DataAtualizacao
            };
        }

        private static DTOUsuarioInfo ToUsuarioInfoDto(UserModel user)
        {
            if (user == null) return null;

            return new DTOUsuarioInfo
            {
                Id = user.Id,
                Usuario = user.Usuario,
                Email = user.Email,
                PermissaoConta = user.PermissionAccount,
                EmpresaContratante = user.EmpresaContratante,
                Plano = user.PlanoPago,
                LimiteAdmin = user.LimiteAdmin,
                LimiteUsuario = user.LimiteUsuario,
                DataCriacao = user.DataCriacao,
                UltimoLogin = user.UltimoLogin,
                Ativo = user.Ativo
            };
        }
    }
}