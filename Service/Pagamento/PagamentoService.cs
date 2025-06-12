using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VoxDocs.DTO;
using VoxDocs.Models;

namespace VoxDocs.Services
{
    public class PagamentoService : IPagamentoService
    {
        private readonly IPagamentoRepository _repository;
        private readonly IUserService _userService;
        private readonly IPagamentoBusinessRules _businessRules;

        public PagamentoService(
            IPagamentoRepository repository,
            IUserService userService,
            IPagamentoBusinessRules businessRules)
        {
            _repository = repository;
            _userService = userService;
            _businessRules = businessRules;
        }

        public async Task<string> CriarPlanoNome(string nomePlanoPlain, string periodicidade)
        {
            // Valida se o plano existe
            await _businessRules.ValidarPlanoExisteAsync(nomePlanoPlain, periodicidade);

            var rawId = Guid.NewGuid().ToString();
            var brasiliaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
            var dataPagamentoBrasilia = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brasiliaTimeZone);

            var pagamento = new PagamentoConcluido
            {
                Id = rawId,
                DataPagamento = dataPagamentoBrasilia,
                NomePlano = nomePlanoPlain,
                Periodicidade = periodicidade
            };

            await _repository.CreatePagamento(pagamento);
            return rawId;
        }

        public async Task<bool> FinalizarPagamentoAsync(FinalizarPagamentoDto dto)
        {
            // Valida pré-requisitos básicos
            await _businessRules.ValidarPagamentoExisteAsync(dto.Id);
            await _businessRules.ValidarDadosPagamentoAsync(dto.Id, dto.NomePlano, dto.Periodicidade);
            await _businessRules.ValidarMetodoPagamentoAsync(dto.MetodoPagamento);

            var brasiliaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
            var dataBrasiliaAgora = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brasiliaTimeZone);

            var existente = await _repository.GetPagamentoById(dto.Id);

            // Atualiza campos do pagamento
            existente.MetodoPagamento = dto.MetodoPagamento;
            existente.DataPagamento = dataBrasiliaAgora;
            existente.DataExpiracao = dataBrasiliaAgora.AddMonths(1);
            existente.StatusEmpresa = "Plano Ativo";
            existente.IsPagamentoConcluido = true;
            existente.EmpresaContratantePlano = dto.EmpresaContratantePlano ?? dto.EmpresaContratante.EmpresaContratante;

            // Verifica e cria empresa
            var empresaExistente = await _repository.GetEmpresaByNome(existente.EmpresaContratantePlano);
            if (empresaExistente == null)
            {
                empresaExistente = new EmpresasContratanteModel
                {
                    EmpresaContratante = existente.EmpresaContratantePlano,
                    Email = dto.EmpresaContratante.Email
                };
                await _repository.CreateEmpresa(empresaExistente);
            }

            // ** Valida existência de empresa e usuários antes de criar tal **
            await _businessRules.ValidarDadosExisteAsync(
                existente.EmpresaContratantePlano,
                dto.Usuarios
            );

            // Cria pastas e subpastas
            foreach (var pastaDto in dto.PastasPrincipais)
            {
                var pasta = new PastaPrincipalModel
                {
                    NomePastaPrincipal = pastaDto.NomePastaPrincipal,
                    EmpresaContratante = pastaDto.EmpresaContratante
                };
                await _repository.CreatePastaPrincipal(pasta);
            }

            foreach (var subpastaDto in dto.SubPastas)
            {
                var subpasta = new SubPastaModel
                {
                    NomeSubPasta = subpastaDto.NomeSubPasta,
                    NomePastaPrincipal = subpastaDto.NomePastaPrincipal,
                    EmpresaContratante = subpastaDto.EmpresaContratante
                };
                await _repository.CreateSubPasta(subpasta);
            }

            // Cria usuários
            foreach (var userDto in dto.Usuarios)
            {
                var dtoUser = new DTORegisterUser
                {
                    Usuario = userDto.Usuario,
                    Email = userDto.Email,
                    Senha = userDto.Senha,
                    PermissionAccount = userDto.PermissionAccount,
                    EmpresaContratante = empresaExistente.EmpresaContratante,
                    PlanoPago = dto.NomePlano
                };

                await _userService.RegisterAsync(dtoUser);
            }

            await _repository.UpdatePagamento(existente);
            return true;
        }

        public async Task<TokenResponseDto> TokenPagoValidoAsync(TokenRequestDto dto)
        {
            return await _businessRules.ValidarTokenPagoAsync(dto);
        }

        public async Task<PlanoInfoDto> ObterDadosPlanoAsync(string token)
        {
            return await _businessRules.ValidarObterDadosPlanoAsync(token);
        }
    }
}
