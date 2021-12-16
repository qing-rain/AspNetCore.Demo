using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QingRain.MvcClient.Implicit.Models;
using System.Diagnostics;

namespace QingRain.MvcClient.Implicit.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
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