using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Collections.Generic;

namespace VoxDocs.Services
{
    public class TokenService
    {
        private static readonly Dictionary<string, string> _activeTokens = new();
        private static readonly string _secretKey = "sua-chave-secreta-aqui"; // Defina sua chave secreta aqui

        // Adiciona ou atualiza o token de um usuário
        public static void AddToken(string usuario, string token)
        {
            _activeTokens[usuario] = token;
        }

        // Remove token usando o valor do token
        public static bool RemoveToken(string token)
        {
            var item = _activeTokens.FirstOrDefault(x => x.Value == token);
            if (!string.IsNullOrEmpty(item.Key))
            {
                _activeTokens.Remove(item.Key);
                return true;
            }
            return false;
        }

        // Verifica se o usuário já tem token ativo
        public static bool IsUserLogged(string usuario)
        {
            return _activeTokens.ContainsKey(usuario);
        }

        // Retorna todos os tokens ativos (opcional)
        public static Dictionary<string, string> GetActiveTokens()
        {
            return _activeTokens;
        }

        // Valida o token JWT e retorna as claims do usuário
        public static IEnumerable<Claim> ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_secretKey); // Sua chave secreta aqui

                // Configura a validação do token
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true, // Verifica se o token não expirou
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero // Configura a tolerância de tempo para expiração (pode ser ajustado)
                };

                // Valida o token e retorna as claims
                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal?.Claims; // Retorna as claims do token
            }
            catch (Exception ex)
            {
                // Adiciona logging ou debug para entender o motivo da falha
                Console.WriteLine($"Erro na validação do token: {ex.Message}");
                return null;
            }
        }
    }
}
