using IDS4Client.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using IdentityModel.Client;

namespace IDS4Client.Controllers
{
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

        public IActionResult JSClient()
        {

            return View();
        }

        public async Task<IActionResult> GetApiData()
        {
            var auth = await HttpContext.AuthenticateAsync();

            var token = auth.Properties.Items[".Token.access_token"];

            var apiClient = new HttpClient();
            apiClient.SetBearerToken(token);

            var response = await apiClient.GetAsync("http://localhost:5153/WeatherForecast");
            string result;
            if (!response.IsSuccessStatusCode)
            {
                result = response.StatusCode.ToString();
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                result = content;// JArray.Parse(content);
            }
            return Json(result);
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