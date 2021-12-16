using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QingRain.MvcClient.AuthorizationCode.Models;
using System.Diagnostics;
using System.Linq;

namespace QingRain.MvcClient.AuthorizationCode
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            string? accessToken = HttpContext.GetTokenAsync("access_token").Result;

            string? idToken = HttpContext.GetTokenAsync("id_token").Result;

            var claimsList = from c in User.Claims select new { c.Type, c.Value };

            return View();
        }

        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}