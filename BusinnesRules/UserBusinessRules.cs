// BusinessRules/UserBusinessRules.cs
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using VoxDocs.DTO;

namespace VoxDocs.BusinessRules
{
    public class UserBusinessRules
    {
        public void ValidateUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Usuário é obrigatório.");
            if (username.Length > 20)
                throw new ArgumentException("Usuário deve ter no máximo 20 caracteres.");
            if (!username.All(char.IsLetter))
                throw new ArgumentException("Usuário deve conter apenas letras.");
        }
        public void ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8)
                throw new ArgumentException("Senha deve ter no mínimo 8 caracteres.");
            if (!password.Any(char.IsUpper))
                throw new ArgumentException("Senha deve conter pelo menos uma letra maiúscula.");
            if (!password.Any(char.IsLower))
                throw new ArgumentException("Senha deve conter pelo menos uma letra minúscula.");
            const string special = "!@#$%^&*()_-+=[]{}|;:'\",.<>?/\\`~";
            if (!password.Any(ch => special.Contains(ch)))
                throw new ArgumentException("Senha deve conter pelo menos um caractere especial.");
        }
        public void ValidatePermissionAccount(string permission)
        {
            if (string.IsNullOrWhiteSpace(permission))
                throw new ArgumentException("PermissionAccount é obrigatório.");
            var p = permission.ToLowerInvariant();
            if (p != "admin" && p != "user")
                throw new ArgumentException("PermissionAccount deve ser 'admin' ou 'user'.");
        }
        public void ValidateUserDto(DTOUser userDto)
        {
            if (userDto == null)
                throw new ArgumentException("Dados de usuário inválidos.");
            ValidateUsername(userDto.Usuario);
            ValidatePassword(userDto.Senha);
            ValidatePermissionAccount(userDto.PermissionAccount);
        }
        public void ValidateLoginDto(DTOUserLogin dto)
        {
            if (dto == null)
                throw new ArgumentException("Dados de login inválidos.");
            if (string.IsNullOrWhiteSpace(dto.Usuario))
                throw new ArgumentException("Usuário é obrigatório.");
            if (string.IsNullOrWhiteSpace(dto.Senha))
                throw new ArgumentException("Senha é obrigatória.");
        }
        public void ValidateRegister(DTOUser dto)
        {
            // Converte os valores para minúsculas
            dto.Usuario = dto.Usuario?.Trim().ToLowerInvariant();
            dto.PermissionAccount = dto.PermissionAccount?.Trim().ToLowerInvariant();

            // Validação de permissões
            if (dto.PermissionAccount != "user" && dto.PermissionAccount != "admin")
            {
                throw new ArgumentException("Tipo de permissão inválido. Apenas 'user' ou 'admin' são aceitos.");
            }

            // Qualquer outra validação adicional que queira adicionar
            // Exemplo: se o nome de usuário for nulo ou vazio
            if (string.IsNullOrEmpty(dto.Usuario))
            {
                throw new ArgumentException("Nome de usuário não pode ser vazio.");
            }
        }
        public void ValidateUpdate(DTOUser dto)
        {
            if (dto == null)
                throw new ArgumentException("Dados do usuário são obrigatórios.");

            if (dto.Id <= 0)
                throw new ArgumentException("ID inválido para atualização.");

            if (string.IsNullOrWhiteSpace(dto.Usuario))
                throw new ArgumentException("Usuário é obrigatório.");

            if (string.IsNullOrWhiteSpace(dto.PermissionAccount))
                throw new ArgumentException("Permissão é obrigatória.");

            dto.Usuario = dto.Usuario.Trim().ToLowerInvariant();
            dto.PermissionAccount = dto.PermissionAccount.Trim().ToLowerInvariant();

            if (dto.PermissionAccount != "user" && dto.PermissionAccount != "admin")
                throw new ArgumentException("Permissão inválida. Apenas 'user' ou 'admin' são aceitos.");
        }
        public void ValidateDelete(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID inválido para exclusão.");
        }
        public bool ValidateActiveToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            try
            {
                var jwt = handler.ReadToken(token) as JwtSecurityToken;
                if (jwt == null) return false;

                return jwt.ValidTo > DateTime.UtcNow;
            }
            catch
            {
                return false;
            }
        }

    }
}