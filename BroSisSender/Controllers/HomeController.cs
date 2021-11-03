using BroSisSender.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BroSisSender.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;
        private static string _lastMessage;
        private static int _broMessageCount = 0;
        private static int _sisMessageCount = 0;
       
        public HomeController(ILogger<HomeController> logger, IAuthenticationSchemeProvider authenticationSchemeProvider)
        {
            _logger = logger;
            _authenticationSchemeProvider = authenticationSchemeProvider;
        }
        public static string LastMessage
        {
            get
            {
                return _lastMessage;
            }
        }
        public static int BroMessageCount
        {
            get
            {
                return _broMessageCount;
            }
        }
        public static int SisMessageCount
        {
            get
            {
                return _sisMessageCount;
            }
        }


        public IActionResult Index()
        {
            return View();
        }
        
        public async Task<IActionResult> Login()
        {
            var allSchemeProvider = (await _authenticationSchemeProvider.GetAllSchemesAsync())
                .Select(n => n.DisplayName).Where(n => !String.IsNullOrEmpty(n));
            return View(allSchemeProvider);
        }

        [Route("Home/GoogleLogin")]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("Index") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [Route("Home/FacebookLogin")]
        public IActionResult FacebookLogin()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("Index") };
            return Challenge(properties, FacebookDefaults.AuthenticationScheme);
        }

        [Route("Home/LogOut")]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("Home/CheckMessage")]
        public IActionResult CheckMessage(string button)
        {
            if (button == "Bro!")
            {
                ViewData["ButtonValue"] = "Bro!";
                _lastMessage = $"Bro!, {User.Identity.Name}";
                _broMessageCount++;
            }
            else
            {
                ViewData["ButtonValue"] = "Sis!";
                _lastMessage = $"Sis!, {User.Identity.Name}";
                _sisMessageCount++;
            }
            return View("Index");
        }
    }
}
