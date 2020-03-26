

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Microsoft.Graph;

namespace _02_webapp.Controllers
{
    [Authorize]
    public class FileController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly GraphServiceClient _client;
        
        public FileController(ILogger<UserController> logger, GraphServiceClient graphServiceClient ) {
            _logger             = logger;
            _client = graphServiceClient;
        }

        public IActionResult Index()
        {
             // request 1 - get user's files
            var request = _client.Me.Drive.Root.Children.Request();

            var results = request.GetAsync().Result;
            return View(results);
        }
    }
}