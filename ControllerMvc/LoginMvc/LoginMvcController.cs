using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoxDocs.DTO;
using VoxDocs.Services;

namespace VoxDocs.Controllers
{
    public class LoginMvcController : Controller
    {
        private readonly ILogger<LoginMvcController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public LoginMvcController(
            ILogger<LoginMvcController> logger,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration, IUserService userService)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _userService = userService;
        }

        [HttpGet, AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult RecuperarSenha()
        {
            return View();
        }

        public IActionResult ConfirmarEmail()
        {
            return View();
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Login([FromForm] DTOLoginUser model)
        {
            if (!ModelState.IsValid)
            {
                TempData["LoginError"] = "Preencha todos os campos corretamente.";
                return View(model);
            }

            try
            {
                // 🔐 Usa o UserService com validações e regras de negócio
                var principal = await _userService.AuthenticateAsync(model);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties { IsPersistent = false }
                );

                return RedirectToAction("Documentos", "DocumentosMvc");
            }
            catch (KeyNotFoundException)
            {
                TempData["LoginError"] = "Conta inexistente.";
            }
            catch (UnauthorizedAccessException)
            {
                TempData["LoginError"] = "Usuário ou senha incorretos.";
            }
            catch (ArgumentException ex)
            {
                // erros de validação (ex: usuário em branco, senha muito curta etc.)
                TempData["LoginError"] = ex.Message;
            }
            catch (Exception)
            {
                TempData["LoginError"] = "Erro inesperado ao fazer login. Tente novamente mais tarde.";
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize] // Protege o logout para usuários autenticados
        public async Task<IActionResult> Logout()
        {
            var client = _httpClientFactory.CreateClient("VoxDocsApi");

            try
            {
                var response = await client.PostAsync("/api/User/Logout", null);
                if (!response.IsSuccessStatusCode)
                {
                    TempData["LogoutError"] = "Erro ao sair. Tente novamente.";
                }
            }
            catch (Exception)
            {
                TempData["LogoutError"] = "Erro ao sair. Tente novamente.";
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }

            HttpContext.Session.Clear();

            return RedirectToAction("Login", "LoginMvc");
        }


        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> RecuperarSenha(string Usuario, string SenhaAntiga, string NovaSenha)
        {
            var client = _httpClientFactory.CreateClient("VoxDocsApi");
            var dto = new
            {
                Usuario = Usuario,
                SenhaAntiga = SenhaAntiga,
                NovaSenha = NovaSenha
            };

            try
            {
                var response = await client.PostAsJsonAsync("/api/User/UpdatePassword", dto);

                if (response.IsSuccessStatusCode)
                {
                    TempData["RecuperarSenhaSuccess"] = "Senha alterada com sucesso!";
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    TempData["RecuperarSenhaError"] = "Usuário não encontrado.";
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    TempData["RecuperarSenhaError"] = "Senha atual incorreta.";
                }
                else
                {
                    var msg = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Erro ao alterar senha: {msg}", msg);
                    TempData["RecuperarSenhaError"] = "Não foi possível alterar a senha. Tente novamente mais tarde.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao tentar alterar senha.");
                TempData["RecuperarSenhaError"] = "Ocorreu um erro inesperado. Tente novamente mais tarde.";
            }

            return RedirectToAction("RecuperarSenha");
        }

        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> RecuperarEmail(string email)
        {
            _logger.LogInformation("[RecuperarEmail] Iniciando envio para: {email}", email);

            try
            {
                var client = _httpClientFactory.CreateClient("VoxDocsApi");
                var response = await client.PostAsJsonAsync("/api/User/GeneratePasswordResetLink", email);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
                    string resetLink = result.GetProperty("link").GetString();

                    // Envio do e-mail com o link real
                    var smtpSection = _configuration.GetSection("Smtp");
                    var host = smtpSection.GetValue<string>("Host");
                    var port = smtpSection.GetValue<int>("Port");
                    var user = smtpSection.GetValue<string>("User");
                    var pass = smtpSection.GetValue<string>("Pass");

                    var smtpClient = new SmtpClient(host)
                    {
                        Port = port,
                        Credentials = new NetworkCredential(user, pass),
                        EnableSsl = true,
                    };

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(user),
                        Subject = "Recuperação de Senha - VoxDocs",
                        Body = $@"Olá,

        Recebemos uma solicitação para redefinir a senha da sua conta VoxDocs.

        Para criar uma nova senha, clique no link abaixo:
        {resetLink}

        Se você não solicitou a redefinição, ignore este e-mail.

        Atenciosamente,
        Equipe VoxDocs",
                        IsBodyHtml = false,
                    };
                    mailMessage.To.Add(email);

                    smtpClient.Send(mailMessage);

                    TempData["LoginSuccess"] = $"Instrução enviada ao e-mail <b>{email}</b>. Por favor, verifique sua caixa de entrada.";
                }
                else
                {
                    TempData["LoginError"] = "Conta inexistente ou e-mail incorreto.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[RecuperarEmail] Erro ao enviar e-mail para: {email}", email);
                TempData["LoginError"] = "Não foi possível enviar o e-mail. Tente novamente mais tarde.";
            }

            return RedirectToAction("Login");
        }

        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> LinkRedefinirSenha(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("ErrorTokenInvalido", "ErrorMvc");
            }

            var client = _httpClientFactory.CreateClient("VoxDocsApi");
            var response = await client.GetAsync($"/api/User/GetTokenExpiration?token={token}");

            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction("ErrorTokenInvalido", "ErrorMvc");
            }

            DateTime? expiration = null;
            var result = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
            var expirationStr = result.GetProperty("expiration").GetString();
            expiration = DateTime.Parse(expirationStr).ToLocalTime();

            ViewBag.Token = token;
            ViewBag.Expiration = expiration;
            return View();
        }
        
        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> LinkRedefinirSenha(string token, string NovaSenha, string ConfirmarSenha)
        {
            if (string.IsNullOrEmpty(NovaSenha) || NovaSenha != ConfirmarSenha)
            {
                TempData["ResetError"] = "As senhas não coincidem.";
                ViewBag.Token = token;
                return View();
            }

            var client = _httpClientFactory.CreateClient("VoxDocsApi");
            var dto = new { Token = token, NovaSenha = NovaSenha };
            var response = await client.PostAsJsonAsync("/api/User/ResetPasswordWithToken", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["LoginSuccess"] = "Senha redefinida com sucesso! Faça login com sua nova senha.";
                return RedirectToAction("Login");
            }
            else
            {
                var msg = await response.Content.ReadAsStringAsync();
                TempData["ResetError"] = "Não foi possível redefinir a senha. " + msg;
                ViewBag.Token = token;
                return View();
            }
        }
    }
}