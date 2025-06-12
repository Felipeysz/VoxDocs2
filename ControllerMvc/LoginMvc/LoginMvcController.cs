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
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public LoginMvcController(
            ILogger<LoginMvcController> logger,
            IUserService userService,
            IConfiguration configuration)
        {
            _logger = logger;
            _userService = userService;
            _configuration = configuration;
        }

        [HttpGet, AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet, AllowAnonymous]
        public IActionResult RecuperarSenha()
        {
            return View();
        }

        [HttpGet, AllowAnonymous]
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
                var principal = await _userService.AuthenticateUserAsync(model);

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
            catch (Exception)
            {
                TempData["LoginError"] = "Erro inesperado ao fazer login. Tente novamente mais tarde.";
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.Session.Clear();
                return RedirectToAction("Login");
            }
            catch (Exception)
            {
                TempData["LogoutError"] = "Erro ao sair. Tente novamente.";
                return RedirectToAction("Documentos", "DocumentosMvc");
            }
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> RecuperarSenha([FromForm] DTOUserLoginPasswordChange model)
        {
            if (!ModelState.IsValid)
            {
                TempData["RecuperarSenhaError"] = "Preencha todos os campos corretamente.";
                return View(model);
            }

            try
            {
                await _userService.ChangePasswordAsync(model);
                TempData["RecuperarSenhaSuccess"] = "Senha alterada com sucesso!";
                return RedirectToAction("Login");
            }
            catch (KeyNotFoundException)
            {
                TempData["RecuperarSenhaError"] = "Usuário não encontrado.";
            }
            catch (UnauthorizedAccessException)
            {
                TempData["RecuperarSenhaError"] = "Senha atual incorreta.";
            }
            catch (Exception)
            {
                TempData["RecuperarSenhaError"] = "Ocorreu um erro inesperado. Tente novamente mais tarde.";
            }

            return View(model);
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> RecuperarEmail([FromForm] DTOResetPassword model)
        {
            try
            {
                await _userService.RequestPasswordResetAsync(model);
                
                // Envio do e-mail (simplificado para exemplo)
                var smtpSection = _configuration.GetSection("Smtp");
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpSection["User"]),
                    Subject = "Recuperação de Senha - VoxDocs",
                    Body = "Instruções para redefinir sua senha foram enviadas para seu e-mail.",
                    IsBodyHtml = false,
                };
                mailMessage.To.Add(model.Email);

                new SmtpClient(smtpSection["Host"])
                {
                    Port = int.Parse(smtpSection["Port"]),
                    Credentials = new NetworkCredential(smtpSection["User"], smtpSection["Pass"]),
                    EnableSsl = true,
                }.Send(mailMessage);

                TempData["LoginSuccess"] = $"Instruções enviadas para {model.Email}. Verifique sua caixa de entrada.";
            }
            catch (KeyNotFoundException)
            {
                TempData["LoginError"] = "E-mail não cadastrado.";
            }
            catch (Exception)
            {
                TempData["LoginError"] = "Não foi possível enviar o e-mail. Tente novamente mais tarde.";
            }

            return RedirectToAction("Login");
        }

        [HttpGet, AllowAnonymous]
        public IActionResult LinkRedefinirSenha(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("ErrorTokenInvalido", "ErrorMvc");
            }

            ViewBag.Token = token;
            return View();
        }
        
        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> LinkRedefinirSenha([FromForm] DTOResetPasswordWithToken model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ResetError"] = "Preencha todos os campos corretamente.";
                ViewBag.Token = model.Token;
                return View();
            }

            try
            {
                await _userService.ResetPasswordWithTokenAsync(model);
                TempData["LoginSuccess"] = "Senha redefinida com sucesso! Faça login com sua nova senha.";
                return RedirectToAction("Login");
            }
            catch (KeyNotFoundException)
            {
                TempData["ResetError"] = "Token inválido ou expirado.";
            }
            catch (UnauthorizedAccessException)
            {
                TempData["ResetError"] = "Token expirado.";
            }
            catch (Exception)
            {
                TempData["ResetError"] = "Não foi possível redefinir a senha. Tente novamente mais tarde.";
            }

            ViewBag.Token = model.Token;
            return View();
        }
    }
}