using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using VoxDocs.DTO;
using VoxDocs.Services;

namespace VoxDocs.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize(Policy = "AdminOnly")] // Restringe a apenas administradores
    public class AdminController : ControllerBase
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IAdminStatisticsService _adminStatsService;

        public AdminController(
            ILogger<AdminController> logger,
            IAdminStatisticsService adminStatsService)
        {
            _logger = logger;
            _adminStatsService = adminStatsService;
        }

        [HttpGet]
        public async Task<IActionResult> DashboardStats()
        {
            try
            {
                var stats = await _adminStatsService.GetAdminStatisticsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter estatísticas do dashboard");
                return StatusCode(500, new { mensagem = "Erro interno ao obter estatísticas do dashboard." });
            }
        }

        [HttpGet("{planoNome}")]
        public async Task<IActionResult> PlanStats(string planoNome)
        {
            try
            {
                var stats = await _adminStatsService.GetPlanStatisticsAsync(planoNome);
                return Ok(stats);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao obter estatísticas do plano {planoNome}");
                return StatusCode(500, new { mensagem = "Erro interno ao obter estatísticas do plano." });
            }
        }

        [HttpGet("{empresaNome}")]
        public async Task<IActionResult> CompanyStats(string empresaNome)
        {
            try
            {
                var stats = await _adminStatsService.GetCompanyStatisticsAsync(empresaNome);
                return Ok(stats);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao obter estatísticas da empresa {empresaNome}");
                return StatusCode(500, new { mensagem = "Erro interno ao obter estatísticas da empresa." });
            }
        }

        [HttpGet("recent-users")]
        public async Task<IActionResult> RecentUsers([FromQuery] int limit = 5)
        {
            try
            {
                var stats = await _adminStatsService.GetAdminStatisticsAsync();
                var recentUsers = stats.UsuariosRecentes
                    .OrderByDescending(u => u.DataCriacao)
                    .Take(limit);

                return Ok(recentUsers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter usuários recentes");
                return StatusCode(500, new { mensagem = "Erro interno ao obter usuários recentes." });
            }
        }

        [HttpGet("active-users")]
        public async Task<IActionResult> ActiveUsersCount()
        {
            try
            {
                var stats = await _adminStatsService.GetAdminStatisticsAsync();
                return Ok(new { activeUsers = stats.UsuariosAtivos });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter contagem de usuários ativos");
                return StatusCode(500, new { mensagem = "Erro interno ao obter contagem de usuários ativos." });
            }
        }

        [HttpGet("admin-count")]
        public async Task<IActionResult> AdminUsersCount()
        {
            try
            {
                var stats = await _adminStatsService.GetAdminStatisticsAsync();
                return Ok(new { adminCount = stats.TotalAdministradores });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter contagem de administradores");
                return StatusCode(500, new { mensagem = "Erro interno ao obter contagem de administradores." });
            }
        }

        [HttpGet("plan-usage")]
        public async Task<IActionResult> PlanUsageStats()
        {
            try
            {
                // Esta ação pode ser expandida para retornar estatísticas de uso de todos os planos
                var stats = await _adminStatsService.GetAdminStatisticsAsync();
                return Ok(new { 
                    totalPlans = stats.TotalPlanosAtivos,
                    // Adicione mais estatísticas conforme necessário
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter estatísticas de uso de planos");
                return StatusCode(500, new { mensagem = "Erro interno ao obter estatísticas de uso de planos." });
            }
        }
    }
}