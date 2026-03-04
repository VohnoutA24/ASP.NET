using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_MWC.Controllers
{
    public class UserController : Controller
    {
        // GET: /User/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /User/Register
        [HttpPost]
        public IActionResult Register(string jmeno, string email, string heslo)
        {
            // Tady by normálně bylo uložení do databáze
            // Pro teď jen přesměrujeme na přihlášení
            TempData["Zprava"] = "Registrace proběhla úspěšně! Nyní se přihlaste.";
            return RedirectToAction("Login");
        }

        // GET: /User/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /User/Login
        [HttpPost]
        public IActionResult Login(string email, string heslo)
        {
            // Tady by normálně bylo ověření proti databázi
            // Pro teď jen uložíme do session že je přihlášen
            HttpContext.Session.SetString("Prihlasen", "true");
            HttpContext.Session.SetString("UserJmeno", "Jan Novák");
            HttpContext.Session.SetString("UserEmail", email);
            if (!string.IsNullOrEmpty(heslo)) HttpContext.Session.SetString("UserHeslo", heslo);
            return RedirectToAction("Profil");
        }

        // GET: /User/Profil
        [HttpGet]
        public IActionResult Profil()
        {
            return ProfilDetailLogic(null);
        }

        // POST: /User/Profil
        [HttpPost]
        public IActionResult Profil(string hesloProOdhaleni)
        {
            return ProfilDetailLogic(hesloProOdhaleni);
        }

        private IActionResult ProfilDetailLogic(string hesloProOdhaleni)
        {
            // Kontrola přihlášení
            if (HttpContext.Session.GetString("Prihlasen") != "true")
            {
                return RedirectToAction("Login");
            }

            ViewBag.Jmeno = HttpContext.Session.GetString("UserJmeno");
            string originalEmail = HttpContext.Session.GetString("UserEmail") ?? "";

            // Odhalení e-mailu pomocí hesla
            if (!string.IsNullOrEmpty(hesloProOdhaleni))
            {
                if (hesloProOdhaleni == HttpContext.Session.GetString("UserHeslo"))
                {
                    ViewBag.Email = originalEmail;
                    ViewBag.Odhaleno = true;
                    return View();
                }
                else
                {
                    ViewBag.ChybaHesla = "Nesprávné heslo! Údaje zůstávají skryty.";
                }
            }

            // Maskování e-mailu (jak před @ tak i po @)
            string maskedEmail = originalEmail;
            int atIndex = originalEmail.IndexOf('@');
            if (atIndex > 1)
            {
                string namePart = originalEmail.Substring(0, atIndex);
                string domainPart = originalEmail.Substring(atIndex + 1); // bez @
                
                // Mask name
                if (namePart.Length <= 2)
                    namePart = new string('*', namePart.Length);
                else
                    namePart = namePart[0] + "***" + namePart[namePart.Length - 1];

                // Mask domain
                int dotIndex = domainPart.LastIndexOf('.');
                if (dotIndex > 0)
                {
                    string domainName = domainPart.Substring(0, dotIndex);
                    string extension = domainPart.Substring(dotIndex);
                    
                    if (domainName.Length <= 2)
                        domainName = new string('*', domainName.Length);
                    else
                        domainName = domainName[0] + "***" + domainName[domainName.Length - 1];
                        
                    domainPart = domainName + extension;
                }
                else
                {
                    domainPart = "***";
                }

                maskedEmail = namePart + "@" + domainPart;
            }
            else if (atIndex == 1)
            {
                maskedEmail = "*@" + originalEmail.Substring(atIndex + 1);
            }
            else if (originalEmail.Length > 2)
            {
                maskedEmail = "***" + originalEmail.Substring(originalEmail.Length - 2);
            }

            ViewBag.Email = maskedEmail;
            ViewBag.Odhaleno = false;
            return View();
        }

        // GET: /User/Odhlasit
        public IActionResult Odhlasit()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
