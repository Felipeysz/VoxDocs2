// Services/AdminStatisticsBusinessRules.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using VoxDocs.Data.Repositories;
using VoxDocs.Interfaces;
using VoxDocs.Models;

namespace VoxDocs.Services
{
    public class AdminStatisticsBusinessRules : IAdminStatisticsBusinessRules
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IEmpresasContratanteRepository _empresaRepository;
        private readonly IPlanosVoxDocsRepository _planoRepository;

        public AdminStatisticsBusinessRules(
            IUsuarioRepository usuarioRepository,
            IEmpresasContratanteRepository empresaRepository,
            IPlanosVoxDocsRepository planoRepository)
        {
            _usuarioRepository = usuarioRepository;
            _empresaRepository = empresaRepository;
            _planoRepository = planoRepository;
        }

        public async Task<EstatisticasAdminModel> ObterEstatisticasAdminAsync()
        {
            var todosUsuarios = await _usuarioRepository.ObterTodosUsuariosAsync();
            var usuariosRecentes = await _usuarioRepository.ObterUsuariosRecentesAsync(10);
            var todasEmpresas = await _empresaRepository.GetAllAsync();
            var todosPlanos = await _planoRepository.GetAllPlansAsync();

            return new EstatisticasAdminModel
            {
                TotalUsuarios = todosUsuarios.Count(),
                UsuariosAtivos = await _usuarioRepository.ContarUsuariosAtivosAsync(),
                TotalAdministradores = await _usuarioRepository.ContarAdministradoresAsync(),
                UsuariosRecentes = usuariosRecentes.ToList(),
                TotalEmpresas = todasEmpresas.Count(),
                TotalPlanosAtivos = todosPlanos.Count(p => p.Ativo)
            };
        }

        public async Task<EstatisticasPlanoModel> ObterEstatisticasPlanoAsync(string planoNome)
        {
            var plano = await _planoRepository.GetPlanByNameAsync(planoNome);
            if (plano == null)
            {
                throw new KeyNotFoundException("Plano não encontrado.");
            }

            var usuariosPlano = await _usuarioRepository.ObterUsuariosPorPlanoAsync(planoNome);

            return new EstatisticasPlanoModel
            {
                NomePlano = planoNome,
                TotalUsuarios = usuariosPlano.Count(),
                UsuariosAtivos = usuariosPlano.Count(u => u.Ativo),
                AdministradoresAtivos = usuariosPlano.Count(u => u.Ativo && u.PermissionAccount == "admin"),
                PercentualUso = plano.LimiteUsuario > 0 ? 
                usuariosPlano.Count(u => u.PermissionAccount == "user") / (double)plano.LimiteUsuario * 100 : 0
            };
        }

        public async Task<EstatisticasEmpresaModel> ObterEstatisticasEmpresaAsync(string empresaNome)
        {
            var empresa = await _empresaRepository.GetByNomeAsync(empresaNome);
            if (empresa == null)
            {
                throw new KeyNotFoundException("Empresa não encontrada.");
            }

            var usuariosEmpresa = await _usuarioRepository.ObterUsuariosPorEmpresaAsync(empresaNome);

            return new EstatisticasEmpresaModel
            {
                NomeEmpresa = empresaNome,
                TotalUsuarios = usuariosEmpresa.Count(),
                UsuariosAtivos = usuariosEmpresa.Count(u => u.Ativo),
                PlanoContratado = empresa.PlanoContratado,
                DataContratacao = empresa.DataContratacao,
                DataRenovacao = empresa.DataContratacao.AddMonths(1) // Assumindo renovação Mensal
            };
        }
    }
}