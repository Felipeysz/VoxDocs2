using System;
using System.Linq;
using System.Threading.Tasks;
using VoxDocs.DTO;
using VoxDocs.Services;

namespace VoxDocs.BusinessRules
{
    public class UserBusinessRules
    {
        private readonly IUserService _userService;
        private readonly IEmpresasContratanteService _empresasService;
        private readonly IPlanosVoxDocsService _planosService;

        public UserBusinessRules(
            IUserService userService,
            IEmpresasContratanteService empresasService,
            IPlanosVoxDocsService planosService)
        {
            _userService = userService;
            _empresasService = empresasService;
            _planosService = planosService;
        }

        public async Task ValidateRegisterAsync(DTOUser dto)
        {
            if (dto == null)
                throw new ArgumentException("Dados de usuário inválidos.");

            dto.Usuario = dto.Usuario?.Trim();
            dto.PermissionAccount = dto.PermissionAccount?.Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(dto.Usuario))
                throw new ArgumentException("Usuário é obrigatório.");
            if (dto.Usuario.Length > 20 || !dto.Usuario.All(char.IsLetter))
                throw new ArgumentException("Usuário deve ter até 20 letras apenas.");

            // Verifica se já existe usuário com mesmo nome
            var userExists = (await _userService.GetUsersAsync()).Any(u => u.Usuario == dto.Usuario);
            if (userExists)
                throw new ArgumentException("Já existe uma conta com este nome de usuário.");

            // Verifica se já existe usuário com mesmo e-mail
            var emailExists = (await _userService.GetUsersAsync()).Any(u => u.Email == dto.Email);
            if (emailExists)
                throw new ArgumentException("Já existe uma conta com este e-mail.");

            if (string.IsNullOrWhiteSpace(dto.Senha) || dto.Senha.Length < 8)
                throw new ArgumentException("Senha deve ter no mínimo 8 caracteres.");
            if (!dto.Senha.Any(char.IsUpper) || !dto.Senha.Any(char.IsLower))
                throw new ArgumentException("Senha deve ter letras maiúsculas e minúsculas.");
            const string special = "!@#$%^&*()_-+=[]{}|;:'\",.<>?/\\`~";
            if (!dto.Senha.Any(ch => special.Contains(ch)))
                throw new ArgumentException("Senha deve conter caractere especial.");

            if (dto.PermissionAccount != "admin" && dto.PermissionAccount != "user")
                throw new ArgumentException("PermissionAccount deve ser 'admin' ou 'user'.");

            // Validação da empresa existente
            if (!string.IsNullOrWhiteSpace(dto.EmpresaContratante))
            {
                var empresas = await _empresasService.GetAllAsync();
                var empresaExiste = empresas.Any(e => e.EmpresaContratante.Equals(dto.EmpresaContratante, StringComparison.OrdinalIgnoreCase));
                if (!empresaExiste)
                    throw new ArgumentException("Empresa informada não existe.");
            }

            // Validação do plano existente
            if (!string.IsNullOrWhiteSpace(dto.PlanoPago))
            {
                var planos = await _planosService.GetAllPlansAsync();
                var plano = planos.FirstOrDefault(p => p.Name.Equals(dto.PlanoPago, StringComparison.OrdinalIgnoreCase));
                if (plano == null)
                    throw new ArgumentException("Plano informado não existe.");
            }
        }

        public void ValidateLoginDto(DTOUserLogin dto)
        {
            if (dto == null)
                throw new ArgumentException("Dados de login inválidos.");
            if (string.IsNullOrWhiteSpace(dto.Usuario) || string.IsNullOrWhiteSpace(dto.Senha))
                throw new ArgumentException("Usuário e senha são obrigatórios.");
        }

        public void ValidateUpdate(DTOUser dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Usuario))
                throw new ArgumentException("Dados do usuário inválidos.");

            dto.Usuario = dto.Usuario.Trim();
            if (!string.IsNullOrWhiteSpace(dto.PermissionAccount))
            {
                dto.PermissionAccount = dto.PermissionAccount.Trim().ToLowerInvariant();
                if (dto.PermissionAccount != "admin" && dto.PermissionAccount != "user")
                    throw new ArgumentException("PermissionAccount deve ser 'admin' ou 'user'.");
            }
        }

        public void ValidateDelete(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID inválido para exclusão.");
        }
    }
}