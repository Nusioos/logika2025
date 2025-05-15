using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using logika2025.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Linq;
using Microsoft.AspNetCore.Identity;
namespace logika2025.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TestmainDbContext _context;
        private static List<Testmain> listaOdpowiedzi = new List<Testmain>();
        private static List<int> losowePytaniaIds = new List<int>();
        private static int Punkty = 0;
        private static int IloscPytan = 0;
        private static int currentQuestionIndex = 0;

        public HomeController(ILogger<HomeController> logger, TestmainDbContext context)
        {
            _logger = logger;
            _context = context;
        }
       
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
            }
            return View();
        }


        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }
        [Authorize]
        public IActionResult OpisTestu()
        {
            return View();
        }
        public IActionResult Error404()
        {
            Response.StatusCode = 404;
            return View("Error404");
        }
        [HttpPost]
        public IActionResult Register(string userName, string email, string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                ViewBag.Error = "Hasła nie są takie same.";
                return View();
            }

            if (_context.Users.Any(u => u.Email == email))
            {
                ViewBag.Error = "Użytkownik z takim e-mailem już istnieje.";
                return View();
            }

            if (_context.Users.Any(u => u.UserName == userName))
            {
                ViewBag.Error = "Ta nazwa użytkownika jest już zajęta.";
                return View();
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new User
            {
                UserName = userName,
                Email = email,
                PasswordHash = hashedPassword
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserName", user.UserName); // zapisz nazwę użytkownika
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Tabela()
        {
            var all_wyn = _context.Odp
                .OrderByDescending(x => x.Odp)     
                .ThenBy(x => x.CzasTrwania)      
                .ToList();

            return View(all_wyn);
        }




        public IActionResult Testmain()
        {
            HttpContext.Session.SetString("StartTime", DateTime.UtcNow.ToString("o")); 
            return RedirectToAction("Pytanko");
        }






        /* public IActionResult TestmainForm(Testmain model)
         {
             _context.Odp.Add(model);
             _context.SaveChanges();
             return RedirectToAction("Index");
         }*/
        public IActionResult TestmainForm(Testmain model)
        {
            if (model != null)
            {
                _context.Odp.Add(model);
                _context.SaveChanges();
            }
            return RedirectToAction("LosowePytanie"); // przekierowanie 
        }

        public IActionResult Test()
        {
            return View();
        }
        public IActionResult Wynik(int wynik)
        {
            var startTimeStr = HttpContext.Session.GetString("StartTime");
            if (string.IsNullOrEmpty(startTimeStr))
            {
                return RedirectToAction("Testmain");
            }

            var startTime = DateTime.Parse(startTimeStr).ToUniversalTime();
            var elapsed = DateTime.UtcNow - startTime;
            var remainingSeconds = (int)(TimeSpan.FromMinutes(20) - elapsed).TotalSeconds;

            ViewBag.RemainingSeconds = remainingSeconds;

            return View(wynik);
        }





        public IActionResult Pytanko()
        {
            var startTimeStr = HttpContext.Session.GetString("StartTime");
            if (string.IsNullOrEmpty(startTimeStr))
            {
                return RedirectToAction("Testmain"); // fallback – brak czasu startu
            }

            var startTime = DateTime.Parse(startTimeStr).ToUniversalTime();
            var elapsed = DateTime.UtcNow - startTime;
            var remainingSeconds = (int)(TimeSpan.FromMinutes(20) - elapsed).TotalSeconds;

            if (remainingSeconds <= 0)
            {
                
                int wynik = Punkty; 
                TimeSpan czasTrwania = DateTime.UtcNow - startTime;

                _context.Odp.Add(new Testmain
                {
                    Odp = wynik,
                    CzasTrwania = czasTrwania
                });
                _context.SaveChanges();

                // Resetuj wszystko
                losowePytaniaIds.Clear();
                currentQuestionIndex = 0;
                Punkty = 0;
                IloscPytan = 0;

                return RedirectToAction("Wynik", new { wynik });
            }

            ViewBag.RemainingSeconds = remainingSeconds;

            var pytania = _context.Pytanie.ToList();

            if (losowePytaniaIds.Count == 0)
            {
                losowePytaniaIds = pytania.Select(p => p.Id).OrderBy(x => Guid.NewGuid()).ToList();
                currentQuestionIndex = 0;
                Punkty = 0;
                IloscPytan = 0;
            }

            if (currentQuestionIndex >= losowePytaniaIds.Count || IloscPytan >= 20 || elapsed > TimeSpan.FromMinutes(60))
            {
                int wynik = Punkty; 
                TimeSpan czasTrwania = DateTime.UtcNow - startTime;
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 
                var userName = User.Identity.Name;
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userName))
                {
                    return RedirectToAction("Login", "Account"); 
                }
                _context.Odp.Add(new Testmain
                {
                    Odp = wynik,
                    CzasTrwania = czasTrwania,
                    UserId = userId,
                       UserName = userName 
                });
                _context.SaveChanges();

                
                currentQuestionIndex = 0;
                Punkty = 0;
                IloscPytan = 0;
                losowePytaniaIds.Clear();

                return RedirectToAction("Wynik", new { wynik }); 
            }

            int pytanieId = losowePytaniaIds[currentQuestionIndex++];
            var pytanie = _context.Pytanie.FirstOrDefault(p => p.Id == pytanieId);

            return View(pytanie);
        }













        [HttpPost]
        public IActionResult SprawdzOdpowiedz(int id, List<string> odpowiedz)
        {
            var pytanie = _context.Pytanie.FirstOrDefault(p => p.Id == id);
            if (pytanie == null)
            {
                return Content("Pytanie nie znalezione.");
            }

            IloscPytan++;

            if (pytanie.TypOdpowiedzi == "wybor")
            {
               
                var poprawne = pytanie.PoprawnaOdpowiedz
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .ToList();

              
                var odpowiedziUzytkownika = odpowiedz.Select(x => x.Trim()).ToList();

       
                if (odpowiedziUzytkownika.All(p => poprawne.Contains(p)) &&
                    poprawne.All(p => odpowiedziUzytkownika.Contains(p)))
                {
                    Punkty++;
                }
            }
            else if (pytanie.TypOdpowiedzi == "tekst")
            {
            
                string odpowiedzTekstowa = odpowiedz.FirstOrDefault()?.Trim() ?? "";

                if (!string.IsNullOrWhiteSpace(pytanie.PoprawnaOdpowiedz) &&
                    pytanie.PoprawnaOdpowiedz.Trim().Equals(odpowiedzTekstowa, StringComparison.OrdinalIgnoreCase))
                {
                    Punkty++;
                }
            }


            if (IloscPytan >= 20)
            {
               var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(userEmail))
                {
                    return Content("Użytkownik niezalogowany lub nie znaleziony.");
                }

                var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
                if (user == null)
                {
                    return Content("Użytkownik niezalogowany lub nie znaleziony.");
                }
          
                var wynik = Punkty;
                var startTimeStr = HttpContext.Session.GetString("StartTime");
                TimeSpan czasTrwania = TimeSpan.Zero;
                if (!string.IsNullOrEmpty(startTimeStr))
                {
                    var startTime = DateTime.Parse(startTimeStr).ToUniversalTime();
                    czasTrwania = DateTime.UtcNow - startTime;
                }

                var nowyWynik = new Testmain
                {
                    Odp = wynik,
                    UserId = user.Id,
                    UserName = user.UserName,
                    CzasTrwania = czasTrwania
                };

                _context.Odp.Add(nowyWynik);
                _context.SaveChanges();

                Punkty = 0;
                IloscPytan = 0;
                currentQuestionIndex = 0;

                return RedirectToAction("Wynik", new { wynik });
            }

            return RedirectToAction("Pytanko");
        }









        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
