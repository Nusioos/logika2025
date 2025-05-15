using logika2025.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;

public class AccountController : Controller
{
    private readonly TestmainDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;

    private readonly IConfiguration _config;

    public AccountController(TestmainDbContext context, IPasswordHasher<User> passwordHasher, IConfiguration config)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _config = config;
    }


    [HttpGet]
    public IActionResult Register() => View();

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = _context.Users.SingleOrDefault(u => u.Email == model.Email);
            if (user != null)
            {
                var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
                if (result == PasswordVerificationResult.Success)
                {
                    await SignInUser(user);
                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError("", "Nieprawidłowy email lub hasło.");
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new User
            {
                Email = model.Email,
                UserName = model.UserName,
                PasswordHash = _passwordHasher.HashPassword(null, model.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await SignInUser(user);

            return RedirectToAction("Index", "Home");
        }

        return View(model);
    }

    private async Task SignInUser(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    }

 
    [HttpGet]
    public IActionResult ForgotPassword() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(string email)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == email);
        if (user == null)
        {
            ModelState.AddModelError("", "Nie znaleziono użytkownika.");
            return View();
        }

        var token = Guid.NewGuid().ToString();
        user.ResetToken = token;
        user.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(32);
        await _context.SaveChangesAsync();

        var resetLink = Url.Action("ResetPassword", "Account", new { token = token }, Request.Scheme);
        var body = $"Kliknij, aby zresetować hasło: <a href='{resetLink}'>Resetuj hasło</a>";

        await SendEmailAsync(user.Email, "Resetowanie hasła - Logika2025", body);

        ViewBag.Message = "E-mail z linkiem do resetu został wysłany.";
        return View();
    }

    public IActionResult Error404()
    {
        Response.StatusCode = 404;
        return View("Error404");
    }

    [HttpGet]
    public IActionResult ResetPassword(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            Console.WriteLine("Brak tokenu w URL!");
            return Error404();
        }

       
        var user = _context.Users.FirstOrDefault(u => u.ResetToken == token);
        if (user == null)
        {
            Console.WriteLine($"Brak użytkownika z tokenem: {token}");
            return Error404();
        }

        
        if (user.ResetTokenExpiry < DateTime.UtcNow)
        {
            Console.WriteLine("Token wygasł.");
            return Error404();
        }

        Console.WriteLine($"Token: {token} jest poprawny, wygasa: {user.ResetTokenExpiry}");

        var model = new ResetPasswordViewModel { Token = token };
        return View(model);
    }




    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        var user = _context.Users.FirstOrDefault(u => u.ResetToken == model.Token && u.ResetTokenExpiry > DateTime.UtcNow);
        if (user == null)
        {
            ModelState.AddModelError("", "Nieprawidłowy lub wygasły token.");
            return View(model);
        }

        if (model.NewPassword != model.ConfirmPassword)
        {
            ModelState.AddModelError("", "Hasła się różnią.");
            return View(model);
        }

        user.PasswordHash = _passwordHasher.HashPassword(user, model.NewPassword);
        user.ResetToken = null; 
        user.ResetTokenExpiry = null; 

        await _context.SaveChangesAsync();
        return RedirectToAction("Login");
    }




   
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var client = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential("logika2025kurs@gmail.com", "zuyq itxk lqsf mkis"),
            EnableSsl = true,
        };

        var mail = new MailMessage
        {
            From = new MailAddress("logika2025kurs@gmail.com"),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mail.To.Add(to);

        await client.SendMailAsync(mail);
    }



}
