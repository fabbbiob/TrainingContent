using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using WebPCat.Models;

namespace WebPCat.Controllers
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
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        private async Task<string> GetTokenForUser()
        {
            // get the current user's account ID
            string userObjectId = User.FindFirstValue(Constants.ClaimIds.UserObjectId);
            string tenantId = User.FindFirstValue(Constants.ClaimIds.TenantId);
            var accountIdentifier = $"{userObjectId}.{tenantId}";
            IAccount account = await application.GetAccountAsync(accountIdentifier);

            var authResult = await application.AcquireTokenSilent(scopes, account).ExecuteAsync();
            return authResult.AccessToken;  
        }
    }
}
