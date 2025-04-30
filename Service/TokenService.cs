using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace VoxDocs.Services
{
    public class TokenService
    {
        private static readonly ConcurrentDictionary<string, (string token, DateTime expiration)> _activeTokens = new();
        private static readonly object _lock = new();
        private static readonly string _secretKey = "sua-chave-secreta-aqui";
        private static readonly TimeSpan _tokenLifetime = TimeSpan.FromMinutes(60); // Tempo de expiração do token

        public static bool IsUserLogged(string usuario)
        {
            lock (_lock)
            {
                RemoveExpiredTokens();
                return _activeTokens.ContainsKey(usuario);
            }
        }

        public static bool AddTokenIfNotExists(string usuario, string token)
        {
            lock (_lock)
            {
                RemoveExpiredTokens();

                if (_activeTokens.ContainsKey(usuario))
                    return false;

                _activeTokens[usuario] = (token, DateTime.UtcNow.Add(_tokenLifetime));
                return true;
            }
        }

        public static bool RemoveToken(string token)
        {
            lock (_lock)
            {
                var item = _activeTokens.FirstOrDefault(x => x.Value.token == token);
                if (!string.IsNullOrEmpty(item.Key))
                {
                    _activeTokens.TryRemove(item.Key, out _);
                    return true;
                }
                return false;
            }
        }

        public static Dictionary<string, string> GetActiveTokens()
        {
            lock (_lock)
            {
                RemoveExpiredTokens();
                return _activeTokens.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.token);
            }
        }

        public static IEnumerable<Claim> ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_secretKey);
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal?.Claims;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro na validação do token: {ex.Message}");
                return null;
            }
        }

        private static void RemoveExpiredTokens()
        {
            var now = DateTime.UtcNow;
            var expired = _activeTokens
                .Where(kvp => kvp.Value.expiration < now)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in expired)
            {
                _activeTokens.TryRemove(key, out _);
            }
        }
    }
}
