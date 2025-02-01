# Analiza Securității

## 1. Sistemul de Conturi și Metode de Criptare

Se utilizează un sistem securizat de gestionare a utilizatorilor, bazat pe Identity Framework. Această infrastructură oferă un mecanism robust pentru autentificare și gestionarea conturilor.

ASP.NET Identity utilizează algoritmi de criptare puternici pentru protecția parolelor utilizatorilor. Mai exact, folosește **hashing cu salt (salting) și algoritmul PBKDF2 (Password-Based Key Derivation Function 2)**. Acest proces presupune:
- Aplicarea unui **salt unic** fiecărei parole pentru a preveni atacurile de tip rainbow table.
- Hashing iterativ al parolei, ceea ce face atacurile brute-force mult mai dificile.
- Utilizarea unui număr mare de iterații pentru a crește costul calculului necesar pentru fiecare atac.

Acest sistem de criptare este eficient deoarece **asigură că parolele nu sunt stocate ca text clar (plaintext)** și minimizează riscul de compromitere chiar și dacă baza de date este expusă.

## 2. Sistemul de Utilizatori și Roluri (Authorize)

ASP.NET oferă un model flexibil de gestionare a utilizatorilor și rolurilor prin intermediul **atributului `[Authorize]`**. Acesta permite **restricționarea accesului la anumite resurse** sau controlere, asigurând astfel securitatea la nivel de aplicație. Implementarea se face prin:
- **Autentificare** (Authentication) - verificarea identității utilizatorului.
- **Autorizare** (Authorization) - determinarea nivelului de acces al utilizatorului.

### Exemple de utilizare:
```csharp
[Authorize(Roles = "User,Admin,Artist")]
public IActionResult Index()
```

Prin utilizarea acestui mecanism, **doar utilizatorii autorizați** pot accesa resursele, eliminand riscul accesului neautorizat.

## 3. Sanitizarea Inputurilor pentru Protecție împotriva Atacurilor de Tip Cross-Site Scripting (XSS)

Cross-Site Scripting (XSS) este un atac comun care presupune **inserarea de cod JavaScript malițios** într-un input nesecurizat. ASP.NET oferă mai multe metode pentru prevenirea acestui tip de atac:

### a) **Validarea Inputului**
Utilizarea **atributelor de validare** pentru a restricționa tipul și lungimea inputurilor:
```csharp
[Required(ErrorMessage = "Title is mandatory")]
[StringLength(50, ErrorMessage = "The title can't be more than 50 characters")]
public string Title { get; set; }
```

### b) **Sanitizarea input-ului cu ajutorul Ganss Xss**
Biblioteca Gans Xss de la Microsoft contine un sistem de sanitizare a input-ului ce si-a demonstrat eficienta atat prin timp, fiind dezvoltata de mai bine de 17 ani, cat si prin numarul de utilizatori (aprox. 58 milioane):

```csharp
var sanitizer = new HtmlSanitizer();
search = sanitizer.Sanitize(search);
```

## 4. Protecția împotriva Atacurilor de Tip Cross-Site Request Forgery (CSRF)  

Cross-Site Request Forgery (CSRF) este un atac care constă în **trimiterea de cereri neautorizate** către un site web în numele unui utilizator autentificat.
Acest atac este posibil atunci când un utilizator, într-o sesiune de browser în care este deja autentificat pe un site vulnerabil, vizitează un site malitios.
În acest caz, browserul trimite automat cookie-urile de autentificare ale site-ului vulnerabil, permițând executarea cererilor neautorizate.  

Framework-ul ASP.NET oferă protecție împotriva CSRF prin utilizarea **anti-forgery tokens**.

Când un formular cu atributele `asp-action`/`asp-controller` este generat într-o pagină Razor, un token anti-forgery este inclus automat în formular sub forma unui input de tip `hidden`.
Pentru cererile POST din JavaScript, token-ul trebuie adăugat manual.

Alt token este stocat în cookie-ul utilizatorului. La trimiterea formularului, serverul verifică dacă cele două token-uri sunt o pereche validă.
Deoarece fiecare sesiune și utilizator au o pereche unică de token-uri, atacatorii nu pot obține token-ul altui utilizator, care ar fi necesar pentru un atac CSRF.

Protecția CSRF este activată prin global adăugarea unui filtru în fișierul `Program.cs`.
Toate cererile, cu exceptia celor care nu modifică starea serverului (GET, HEAD, OPTIONS și TRACE), vor fi verificate automat pentru token-ul anti-forgery.

```csharp
builder.Services.AddMvc(options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));
```
