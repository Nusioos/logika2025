using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using logika2025.Models; 
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug() 
    .WriteTo.File("Logs/app_log.txt", rollingInterval: RollingInterval.Day) 
    .CreateLogger();


builder.Host.UseSerilog();

builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();



builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddSerilog();
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; 
        options.LogoutPath = "/Account/Logout"; 
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); 
    });


builder.Services.AddDbContext<TestmainDbContext>(options =>
{
    var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connStr);
    options.EnableSensitiveDataLogging(true);
    options.EnableDetailedErrors(true);
});


var app = builder.Build();
try
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<TestmainDbContext>();
        dbContext.Database.OpenConnection();
        Log.Information("Połączenie z bazą danych zostało pomyślnie nawiązane.");
    }
}
catch (Exception ex)
{
    Log.Error($"Błąd połączenia z bazą danych: {ex.Message}");
}
app.UseSession(); 


using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TestmainDbContext>();

    context.Database.Migrate(); 
    if (!context.Pytanie.Any())
    {
        var pytania = new List<Pytania>
{
    new Pytania
    {
        Tresc = "Jaka jest definicja bijekcji?",
        OdpowiedzA = "To jest bijekcja",
        OdpowiedzB = "To jest injekcja, ale nie suriekcja",
        OdpowiedzC = "To jest suriekcja, ale nie injekcja",
        OdpowiedzD = "Ani injekcja, ani suriekcja",
     
        PoprawnaOdpowiedz = "A", 
        TypOdpowiedzi = "wybor",
        Obraz = "pytanie3.png"
    },
    new Pytania
    {
        Tresc = "Niech f taką, że f(x) = mod(x,3) gdzie mod(a,b) oznacza resztę z dzielenia liczby a przez b. Zaznacz zdanie prawdziwe o f.",
        OdpowiedzA = "f jest funkcją różnowartościową",
        OdpowiedzB = "f jest funkcją na",
        OdpowiedzC = "f jest funkcją różnowartościową i na",
        OdpowiedzD = "f nie jest funkcją różnowartościową ani na",
        PoprawnaOdpowiedz = "D",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Niech A i B będą dowolnymi zbiorami. Zaznacz prawdziwe zdania.",
        OdpowiedzA = "A = B ⇒ |A| = |B|",
        OdpowiedzB = "|A| = |B| ⇒ A⊂B",
        OdpowiedzC = "|A| > |N| ⇒ |A| = |R|",
        PoprawnaOdpowiedz = "A",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Zaznacz pary zbiorów, pomiędzy którymi istnieje bijekcja.",
        OdpowiedzA = "Liczba wymierna (traktowana jako klasa abstrakcji) i Q",
        OdpowiedzB = "Q^2 i R^4",
        OdpowiedzC = "Z i N",
        PoprawnaOdpowiedz = "C",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Zaznacz operacje, które są przemienne.",
        OdpowiedzA = "Dodawanie liczb całkowitych",
        OdpowiedzB = "Mnożenie liczb całkowitych",
        OdpowiedzC = "Odejmowanie liczb całkowitych",
        OdpowiedzD = "Dzielenie liczb całkowitych",
        PoprawnaOdpowiedz = "A,B",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Niech p oznacza zdanie logiczne, a 1 zdanie prawdziwe. Wskaż wszystkie tautologie.",
        OdpowiedzA = "p ∨ ¬p",
        OdpowiedzB = "p ∧ ¬p",
        OdpowiedzC = "¬(p ∧ ¬p)",
        OdpowiedzD = "p ⇒ p",
        PoprawnaOdpowiedz = "A,C,D",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Niech P(X) oznacza zbiór potęgowy zbioru A, a ⊕ oznacza różnicę symetryczną zbiorów. Co jest równe P(A) ⊕ P(B)?",
        OdpowiedzA = "P(A ∪ B)",
        OdpowiedzB = "P(A) ∪ P(B)",
        OdpowiedzC = "P(A ∩ B)",
        OdpowiedzD = "P(A) ∩ P(B)",
        PoprawnaOdpowiedz = "B",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Czy istnieją relacje, które są jednocześnie symetryczne i antysymetryczne?",
        OdpowiedzA = "Tak, np. relacja równości",
        OdpowiedzB = "Nie, to niemożliwe",
        OdpowiedzC = "Tak, np. relacja pusta",
        OdpowiedzD = "Tak, np. relacja pełna",
        PoprawnaOdpowiedz = "A,C",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Zaznacz zdania równoważne zdaniu A ⇔ B.",
        OdpowiedzA = "(A ⇒ B) ∧ (B ⇒ A)",
        OdpowiedzB = "¬A ∨ B",
        OdpowiedzC = "¬(A ∧ ¬B) ∧ ¬(¬A ∧ B)",
        OdpowiedzD = "(A ∧ B) ∨ (¬A ∧ ¬B)",
        PoprawnaOdpowiedz = "A,C,D",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Niech R będzie relacją równoważności na zbiorze X, |X| > 2. Wtedy zachodzi:",
        OdpowiedzA = "Każdy element X należy do dokładnie jednej klasy abstrakcji",
        OdpowiedzB = "Istnieje element X, który należy do więcej niż jednej klasy abstrakcji",
        OdpowiedzC = "Relacja R jest symetryczna, przechodnia i zwrotna",
        OdpowiedzD = "Relacja R jest tylko symetryczna i przechodnia",
        PoprawnaOdpowiedz = "A,C",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Co możemy powiedzieć o liczbie relacji porządku określonych na zbiorze X o co najmniej trzech elementach? Zaznacz zdanie prawdziwe.",
        OdpowiedzA = "Istnieje dokładnie jedna relacja porządku na zbiorze X",
        OdpowiedzB = "Liczba relacji porządku na zbiorze X jest równa liczbie permutacji zbioru X",
        OdpowiedzC = "Liczba relacji porządku na zbiorze X jest większa niż liczba permutacji zbioru X",
        OdpowiedzD = "Nie istnieją relacje porządku na zbiorze X",
        PoprawnaOdpowiedz = "C",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Jeżeli {A_n} oraz {B_n} są zstępującymi rodzinami zbiorów to zachodzi równość: ⋂A_n ∪ ⋂B_n = ⋂(A_n ∪ B_n). Wybierz jedną odpowiedź.",
        OdpowiedzA = "Równość zawsze zachodzi",
        OdpowiedzB = "Równość zachodzi tylko dla skończonych rodzin zbiorów",
        OdpowiedzC = "Równość nie zawsze zachodzi",
        OdpowiedzD = "Równość zachodzi tylko wtedy, gdy A_n = B_n dla każdego n",
        PoprawnaOdpowiedz = "A",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Niech φ(x) będzie formą zdaniową z argumentem przyjmującym wartości należące do niepustego uniwersum, a ψ zdaniem logicznym. Wskaż wszystkie tautologie.",
        OdpowiedzA = "∀x φ(x) ⇒ ∃x φ(x)",
        OdpowiedzB = "∃x φ(x) ⇒ ∀x φ(x)",
        OdpowiedzC = "¬(∃x φ(x)) ⇔ ∀x ¬φ(x)",
        OdpowiedzD = "¬(∀x φ(x)) ⇔ ∃x ¬φ(x)",
        PoprawnaOdpowiedz = "A,C,D",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Niech f: X → Y. Podaj liczbę suriekcji (funkcji 'na') z X do Y, gdzie |X| = 3, |Y| = 2.",
        PoprawnaOdpowiedz = "6",
        TypOdpowiedzi = "tekst"
    },
    new Pytania
    {
        Tresc = "Z poniższych równości wybierz wszystkie będące tożsamościami rachunku zbiorów.",
        OdpowiedzA = "A ∪ (B ∩ C) = (A ∪ B) ∩ (A ∪ C)",
        OdpowiedzB = "A ∩ (B ∪ C) = (A ∩ B) ∪ (A ∩ C)",
        OdpowiedzC = "A \\ (B ∪ C) = (A \\ B) ∩ (A \\ C)",
        OdpowiedzD = "A ∪ (B \\ C) = (A ∪ B) \\ (A ∪ C)",
        PoprawnaOdpowiedz = "A,B,C",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Niech φ(x) będzie formą zdaniową z argumentem przyjmującym wartości należące do niepustego uniwersum. Wskaż wszystkie tautologie.",
        OdpowiedzA = "∀x φ(x) ⇒ ∃x φ(x)",
        OdpowiedzB = "∃x φ(x) ⇒ ∀x φ(x)",
        OdpowiedzC = "¬(∃x φ(x)) ⇔ ∀x ¬φ(x)",
        OdpowiedzD = "¬(∀x φ(x)) ⇔ ∃x ¬φ(x)",
        PoprawnaOdpowiedz = "A,C,D",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Rozważmy funkcję f: X → Y. Obraz zbioru A przez funkcję f oznaczamy jako f(A), a przeciwobraz zbioru B jako f⁻¹(B). Wskaż wszystkie prawdziwe stwierdzenia.",
        OdpowiedzA = "f(f⁻¹(B)) ⊆ B",
        OdpowiedzB = "A ⊆ f⁻¹(f(A))",
        OdpowiedzC = "f(f⁻¹(B)) = B dla każdego B ⊆ Y",
        OdpowiedzD = "A = f⁻¹(f(A)) dla każdego A ⊆ X",
        PoprawnaOdpowiedz = "A,B",
        TypOdpowiedzi = "wybor"
    },
      new Pytania
    {
        Tresc = "Niech f taką, że f(x) = mod(x,3) gdzie mod(a,b) oznacza resztę z dzielenia liczby a przez b. Zaznacz zdanie prawdziwe o f.",
        OdpowiedzA = "f jest funkcją różnowartościową",
        OdpowiedzB = "f jest funkcją na",
        OdpowiedzC = "f jest funkcją różnowartościową i na",
        OdpowiedzD = "f nie jest funkcją różnowartościową ani na",
        PoprawnaOdpowiedz = "B",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Zaznacz operacje, które są przemienne.",
        OdpowiedzA = "Dodawanie liczb całkowitych",
        OdpowiedzB = "Mnożenie liczb całkowitych",
        OdpowiedzC = "Odejmowanie liczb całkowitych",
        OdpowiedzD = "Dzielenie liczb całkowitych",
        PoprawnaOdpowiedz = "A,B",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Niech p oznacza zdanie logiczne, a 1 zdanie prawdziwe. Wskaż wszystkie tautologie.",
        OdpowiedzA = "p ∨ ¬p",
        OdpowiedzB = "p ∧ ¬p",
        OdpowiedzC = "¬(p ∧ ¬p)",
        OdpowiedzD = "p ⇒ p",
        PoprawnaOdpowiedz = "A,C,D",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Niech P(X) oznacza zbiór potęgowy zbioru a o oznacza różnicę symetryczną zbiorów. Wskaż poprawne równości.",
        OdpowiedzA = "P(X) ∪ P(Y) = P(X ∪ Y)",
        OdpowiedzB = "P(X) ∩ P(Y) = P(X ∩ Y)",
        OdpowiedzC = "P(X) \\ P(Y) = P(X \\ Y)",
        OdpowiedzD = "P(X) Δ P(Y) = P(X Δ Y)",
        PoprawnaOdpowiedz = "B",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Czy istnieją relacje, które są jednocześnie symetryczne i antysymetryczne?",
        OdpowiedzA = "Tak, np. relacja równości",
        OdpowiedzB = "Nie, to niemożliwe",
        OdpowiedzC = "Tak, np. relacja pusta",
        OdpowiedzD = "Tak, np. relacja pełna",
        PoprawnaOdpowiedz = "A,C",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Zaznacz zdania równoważne zdaniu a <=> B.",
        OdpowiedzA = "(a ∧ B) ∨ (¬a ∧ ¬B)",
        OdpowiedzB = "¬(a ⊕ B)",
        OdpowiedzC = "(a ⇒ B) ∧ (B ⇒ a)",
        OdpowiedzD = "(a ∨ B) ∧ (¬a ∨ ¬B)",
        PoprawnaOdpowiedz = "A,B,C",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Niech f : Z × Z → Z będzie dane wzorem f(n,k) = n2k. Które z poniższych zdań są prawdziwe?",
        OdpowiedzA = "f jest funkcją różnowartościową",
        OdpowiedzB = "f jest funkcją na",
        OdpowiedzC = "f jest funkcją różnowartościową i na",
        OdpowiedzD = "f nie jest funkcją różnowartościową ani na",
        PoprawnaOdpowiedz = "D",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Niech A, B i C będą zbiorami. Które z poniższych zdań są prawdziwe?",
        OdpowiedzA = "A ∩ (B ∪ C) = (A ∩ B) ∪ (A ∩ C)",
        OdpowiedzB = "A ∪ (B ∩ C) = (A ∪ B) ∩ (A ∪ C)",
        OdpowiedzC = "(A ∩ B) ∩ C = A ∩ (B ∩ C)",
        OdpowiedzD = "A ∩ B = B ∩ A",
        PoprawnaOdpowiedz = "A,B,C,D",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Dla funkcji f: A -> B, zaznacz prawdziwe zdanie o funkcji odwrotnej.",
        OdpowiedzA = "Funkcja odwrotna istnieje tylko wtedy, gdy f jest różnowartościowa",
        OdpowiedzB = "Funkcja odwrotna istnieje tylko wtedy, gdy f jest surjekcją",
        OdpowiedzC = "Funkcja odwrotna zawsze istnieje, jeśli f jest injekcją",
        OdpowiedzD = "Funkcja odwrotna zawsze istnieje, jeśli f jest bijekcją",
        PoprawnaOdpowiedz = "A,D",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Która z poniższych relacji jest relacją porządku częściowego?",
        OdpowiedzA = "Relacja równości na zbiorze liczb całkowitych",
        OdpowiedzB = "Relacja podzbiorem na zbiorze zbiorów",
        OdpowiedzC = "Relacja mniejsze od na zbiorze liczb naturalnych",
        OdpowiedzD = "Relacja mniejsze od na zbiorze liczb całkowitych",
        PoprawnaOdpowiedz = "A,B,C",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Czy funkcja f(x) = x^3 jest różnowartościowa?",
        OdpowiedzA = "Tak",
        OdpowiedzB = "Nie",
        PoprawnaOdpowiedz = "A",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Dla jakiego zbioru A, jego zbiór potęgowy P(A) ma dokładnie 8 elementów?",
        OdpowiedzA = "A = {1, 2}",
        OdpowiedzB = "A = {1, 2, 3}",
        OdpowiedzC = "A = {1, 2, 3, 4}",
        OdpowiedzD = "A = {1}",
        PoprawnaOdpowiedz = "B",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Czy zbiór liczb pierwszych jest zbiorem nieograniczonym?",
        OdpowiedzA = "Tak",
        OdpowiedzB = "Nie",
        PoprawnaOdpowiedz = "A",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Dla zbioru liczb rzeczywistych, zaznacz funkcję, która jest ciągła, ale nie różniczkowalna.",
        OdpowiedzA = "f(x) = x^2",
        OdpowiedzB = "f(x) = |x|",
        OdpowiedzC = "f(x) = sin(x)",
        OdpowiedzD = "f(x) = x^3",
        PoprawnaOdpowiedz = "B",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Zaznacz definicję funkcji surjekcyjnej.",
        OdpowiedzA = "Funkcja, która przypisuje każdemu elementowi z A dokładnie jeden element z B",
        OdpowiedzB = "Funkcja, dla której każde element z B ma przynajmniej jeden element z A",
        OdpowiedzC = "Funkcja, która przypisuje każdemu elementowi z A element z B",
        OdpowiedzD = "Funkcja, która przypisuje każdemu elementowi z A dokładnie jeden element z B, a także odwrotnie",
        PoprawnaOdpowiedz = "B",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Zaznacz, która z poniższych funkcji jest różnowartościowa.",
        OdpowiedzA = "f(x) = x^2, x ∈ R",
        OdpowiedzB = "f(x) = 2x, x ∈ R",
        OdpowiedzC = "f(x) = x + 1, x ∈ R",
        OdpowiedzD = "f(x) = x^2, x ∈ R+",
        PoprawnaOdpowiedz = "B,C",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Które z poniższych zdań o funkcjach są prawdziwe?",
        OdpowiedzA = "Każda funkcja injektywna jest surjekcyjna",
        OdpowiedzB = "Każda funkcja surjekcyjna jest injektywna",
        OdpowiedzC = "Każda funkcja bijekcyjna jest zarówno injektywna, jak i surjekcyjna",
        OdpowiedzD = "Funkcja injektywna nie jest zawsze bijekcyjna",
        PoprawnaOdpowiedz = "C,D",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Niech A, B i C będą zbiorami. Które z poniższych zdań są prawdziwe?",
        OdpowiedzA = "A ∩ (B ∪ C) = (A ∩ B) ∪ (A ∩ C)",
        OdpowiedzB = "A ∪ (B ∩ C) = (A ∪ B) ∩ (A ∪ C)",
        OdpowiedzC = "(A ∩ B) ∩ C = A ∩ (B ∩ C)",
        OdpowiedzD = "A ∩ B = B ∩ A",
        PoprawnaOdpowiedz = "A,B,C,D",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Dla funkcji f: A -> B, zaznacz prawdziwe zdanie o funkcji odwrotnej.",
        OdpowiedzA = "Funkcja odwrotna istnieje tylko wtedy, gdy f jest różnowartościowa",
        OdpowiedzB = "Funkcja odwrotna istnieje tylko wtedy, gdy f jest surjekcją",
        OdpowiedzC = "Funkcja odwrotna zawsze istnieje, jeśli f jest injekcją",
        OdpowiedzD = "Funkcja odwrotna zawsze istnieje, jeśli f jest bijekcją",
        PoprawnaOdpowiedz = "A,D",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Która z poniższych relacji jest relacją porządku częściowego?",
        OdpowiedzA = "Relacja równości na zbiorze liczb całkowitych",
        OdpowiedzB = "Relacja podzbiorem na zbiorze zbiorów",
        OdpowiedzC = "Relacja mniejsze od na zbiorze liczb naturalnych",
        OdpowiedzD = "Relacja mniejsze od na zbiorze liczb całkowitych",
        PoprawnaOdpowiedz = "A,B,C",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Czy funkcja f(x) = x^3 jest różnowartościowa?",
        OdpowiedzA = "Tak",
        OdpowiedzB = "Nie",
        PoprawnaOdpowiedz = "A",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Dla jakiego zbioru A, jego zbiór potęgowy P(A) ma dokładnie 8 elementów?",
        OdpowiedzA = "A = {1, 2}",
        OdpowiedzB = "A = {1, 2, 3}",
        OdpowiedzC = "A = {1, 2, 3, 4}",
        OdpowiedzD = "A = {1}",
        PoprawnaOdpowiedz = "B",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Czy zbiór liczb pierwszych jest zbiorem nieograniczonym?",
        OdpowiedzA = "Tak",
        OdpowiedzB = "Nie",
        PoprawnaOdpowiedz = "A",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Dla zbioru liczb rzeczywistych, zaznacz funkcję, która jest ciągła, ale nie różniczkowalna.",
        OdpowiedzA = "f(x) = x^2",
        OdpowiedzB = "f(x) = |x|",
        OdpowiedzC = "f(x) = sin(x)",
        OdpowiedzD = "f(x) = x^3",
        PoprawnaOdpowiedz = "B",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Zaznacz definicję funkcji surjekcyjnej.",
        OdpowiedzA = "Funkcja, która przypisuje każdemu elementowi z A dokładnie jeden element z B",
        OdpowiedzB = "Funkcja, dla której każde element z B ma przynajmniej jeden element z A",
        OdpowiedzC = "Funkcja, która przypisuje każdemu elementowi z A element z B",
        OdpowiedzD = "Funkcja, która przypisuje każdemu elementowi z A dokładnie jeden element z B, a także odwrotnie",
        PoprawnaOdpowiedz = "B",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Zaznacz, która z poniższych funkcji jest różnowartościowa.",
        OdpowiedzA = "f(x) = x^2, x ∈ R",
        OdpowiedzB = "f(x) = 2x, x ∈ R",
        OdpowiedzC = "f(x) = x + 1, x ∈ R",
        OdpowiedzD = "f(x) = x^3, x ∈ R+",
        PoprawnaOdpowiedz = "B,C",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Które z poniższych zdań o funkcjach są prawdziwe?",
        OdpowiedzA = "Każda funkcja injektywna jest surjekcyjna",
        OdpowiedzB = "Każda funkcja surjekcyjna jest injektywna",
        OdpowiedzC = "Każda funkcja bijekcyjna jest zarówno injektywna, jak i surjekcyjna",
        OdpowiedzD = "Funkcja injektywna nie jest zawsze bijekcyjna",
        PoprawnaOdpowiedz = "C,D",
        TypOdpowiedzi = "wybor"
    },
     new Pytania
    {
        Tresc = "Czy każda funkcja różnowartościowa jest funkcją na?",
        OdpowiedzA = "Tak",
        OdpowiedzB = "Nie",
        PoprawnaOdpowiedz = "B",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Jaka jest definicja funkcji surjekcyjnej?",
        OdpowiedzA = "Funkcja, która przypisuje każdemu elementowi zbioru A dokładnie jeden element zbioru B",
        OdpowiedzB = "Funkcja, której zbiór wartości pokrywa cały zbiór B",
        OdpowiedzC = "Funkcja, która przypisuje każdemu elementowi zbioru A różne elementy zbioru B",
        OdpowiedzD = "Funkcja, która jest bijekcją",
        PoprawnaOdpowiedz = "B",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Rozważmy funkcję f: A -> B. Które z poniższych warunków zapewniają, że f jest surjekcją?",
        OdpowiedzA = "Każdy element zbioru B ma przypisany element z A",
        OdpowiedzB = "Każdy element zbioru A ma przypisany element z B",
        OdpowiedzC = "Funkcja jest różnowartościowa",
        OdpowiedzD = "Funkcja jest bijekcją",
        PoprawnaOdpowiedz = "A",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Czy funkcja f(x) = x^2, x ∈ R jest funkcją różnowartościową?",
        OdpowiedzA = "Tak",
        OdpowiedzB = "Nie",
        PoprawnaOdpowiedz = "B",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Ile elementów ma zbiór potęgowy zbioru A = {1, 2, 3, 4, 5}?",
        PoprawnaOdpowiedz = "32",
        TypOdpowiedzi = "tekst"
    },
    new Pytania
    {
        Tresc = "Jakie zbiory mają tę samą moc?",
        OdpowiedzA = "Zbiór liczb całkowitych i zbiór liczb naturalnych",
        OdpowiedzB = "Zbiór liczb rzeczywistych i zbiór liczb wymiernych",
        OdpowiedzC = "Zbiór liczb całkowitych i zbiór liczb rzeczywistych",
        OdpowiedzD = "Zbiór liczb wymiernych i zbiór liczb naturalnych",
        PoprawnaOdpowiedz = "A",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Co to jest funkcja bijekcyjna?",
        OdpowiedzA = "Funkcja, która jest zarówno injekcją, jak i surjekcją",
        OdpowiedzB = "Funkcja, która jest injekcją",
        OdpowiedzC = "Funkcja, która jest surjekcją",
        OdpowiedzD = "Funkcja, która jest różnowartościowa",
        PoprawnaOdpowiedz = "A",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Zaznacz prawdziwe zdanie o funkcji f: R -> R, f(x) = x^3.",
        OdpowiedzA = "Funkcja jest różnowartościowa",
        OdpowiedzB = "Funkcja nie jest surjekcją",
        OdpowiedzC = "Funkcja jest bijekcją",
        OdpowiedzD = "Funkcja nie jest funkcją parzystą",
        PoprawnaOdpowiedz = "A,C,D",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
    {
        Tresc = "Jeśli funkcja f jest bijekcją, to jaką funkcję posiada?",
        OdpowiedzA = "Odwrotną",
        OdpowiedzB = "Surjekcyjną",
        OdpowiedzC = "Injekcyjną",
        OdpowiedzD = "Każdą funkcję",
        PoprawnaOdpowiedz = "A",
        TypOdpowiedzi = "wybor"
    },
    new Pytania
{
    Tresc = "Niech f będzie funkcją określoną wzorem f(x) = x^2. Zaznacz prawdziwe zdanie o funkcji f.",
    OdpowiedzA = "Funkcja jest różnowartościowa",
    OdpowiedzB = "Funkcja jest parzysta",
    OdpowiedzC = "Funkcja jest nieparzysta",
    OdpowiedzD = "Funkcja jest bijekcją",
    PoprawnaOdpowiedz = "B",
    TypOdpowiedzi = "wybor"
},
new Pytania
{
    Tresc = "Zbiór A = {a, b, c, d} ma jaką moc?",
    OdpowiedzA = "2",
    OdpowiedzB = "3",
    OdpowiedzC = "4",
    OdpowiedzD = "5",
    PoprawnaOdpowiedz = "C",
    TypOdpowiedzi = "wybor"
},
new Pytania
{
    Tresc = "Jeśli funkcja f jest różnowartościowa i surjektywna, to jest:",
    OdpowiedzA = "Iniekcją",
    OdpowiedzB = "Bijekcją",
    OdpowiedzC = "Surjekcją",
    OdpowiedzD = "Funkcją stałą",
    PoprawnaOdpowiedz = "B",
    TypOdpowiedzi = "wybor"
},
new Pytania
{
    Tresc = "Niech A = {1, 2, 3}, B = {a, b}. Ile funkcji różnowartościowych można zdefiniować z A do B?",
    OdpowiedzA = "0",
    OdpowiedzB = "2",
    OdpowiedzC = "6",
    OdpowiedzD = "8",
    PoprawnaOdpowiedz = "A",
    TypOdpowiedzi = "wybor"
},
new Pytania
{
    Tresc = "Funkcja f: R → R, f(x) = x^3 jest:",
    OdpowiedzA = "Parzysta",
    OdpowiedzB = "Nieparzysta",
    OdpowiedzC = "Stała",
    OdpowiedzD = "Żadna z powyższych",
    PoprawnaOdpowiedz = "B",
    TypOdpowiedzi = "wybor"
},
new Pytania
{
    Tresc = "Zbiór liczb naturalnych jest:",
    OdpowiedzA = "Skończony",
    OdpowiedzB = "Nieskończony przeliczalny",
    OdpowiedzC = "Nieskończony nieprzeliczalny",
    OdpowiedzD = "Pusty",
    PoprawnaOdpowiedz = "B",
    TypOdpowiedzi = "wybor"
},
new Pytania
{
    Tresc = "Funkcja f: R → R, f(x) = sin(x) jest:",
    OdpowiedzA = "Parzysta",
    OdpowiedzB = "Nieparzysta",
    OdpowiedzC = "Bijekcją",
    OdpowiedzD = "Stała",
    PoprawnaOdpowiedz = "B",
    TypOdpowiedzi = "wybor"
},
new Pytania
{
    Tresc = "Jeśli funkcja f jest parzysta, to dla każdego x zachodzi:",
    OdpowiedzA = "f(x) = f(-x)",
    OdpowiedzB = "f(x) = -f(-x)",
    OdpowiedzC = "f(-x) = -f(x)",
    OdpowiedzD = "f(x) = 0",
    PoprawnaOdpowiedz = "A",
    TypOdpowiedzi = "wybor"
},
new Pytania
{
    Tresc = "Ile jest funkcji ze zbioru A = {1,2} do zbioru B = {a,b,c}?",
    OdpowiedzA = "6",
    OdpowiedzB = "8",
    OdpowiedzC = "9",
    OdpowiedzD = "12",
    PoprawnaOdpowiedz = "C",
    TypOdpowiedzi = "wybor"
},
new Pytania
{
    Tresc = "Funkcja f: R → R, f(x) = |x| jest:",
    OdpowiedzA = "Parzysta",
    OdpowiedzB = "Nieparzysta",
    OdpowiedzC = "Bijekcją",
    OdpowiedzD = "Stała",
    PoprawnaOdpowiedz = "A",
    TypOdpowiedzi = "wybor"
},
new Pytania
{
    Tresc = "Ile jest permutacji zbioru {1, 2, 3, 4}?",
    OdpowiedzA = "24",
    OdpowiedzB = "16",
    OdpowiedzC = "8",
    OdpowiedzD = "4",
    PoprawnaOdpowiedz = "A",
    TypOdpowiedzi = "wybor"
},
new Pytania
{
    Tresc = "Jeśli A ⊆ B i B ⊆ C, to:",
    OdpowiedzA = "A ⊆ C",
    OdpowiedzB = "C ⊆ A",
    OdpowiedzC = "A = C",
    OdpowiedzD = "A ∩ C = ∅",
    PoprawnaOdpowiedz = "A",
    TypOdpowiedzi = "wybor"
},
new Pytania
{
    Tresc = "Ile podzbiorów ma zbiór {a, b, c}?",
    OdpowiedzA = "6",
    OdpowiedzB = "8",
    OdpowiedzC = "9",
    OdpowiedzD = "7",
    PoprawnaOdpowiedz = "B",
    TypOdpowiedzi = "wybor"
},
new Pytania
{
    Tresc = "Funkcja f: R → R, f(x) = e^x jest:",
    OdpowiedzA = "Parzysta",
    OdpowiedzB = "Nieparzysta",
    OdpowiedzC = "Ani parzysta, ani nieparzysta",
    OdpowiedzD = "Stała",
    PoprawnaOdpowiedz = "C",
    TypOdpowiedzi = "wybor"
},
new Pytania
{
    Tresc = "Jeśli funkcja f jest różnowartościowa, to dla każdego x1 ≠ x2 zachodzi:",
    OdpowiedzA = "f(x1) = f(x2)",
    OdpowiedzB = "f(x1) ≠ f(x2)",
    OdpowiedzC = "f(x1) > f(x2)",
    OdpowiedzD = "f(x1) < f(x2)",
    PoprawnaOdpowiedz = "B",
    TypOdpowiedzi = "wybor"
}
};


        context.Pytanie.AddRange(pytania); 
        context.SaveChanges(); 
    }
}


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession(); 
app.UseRouting();

app.UseAuthentication();  
app.UseAuthorization();  

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
