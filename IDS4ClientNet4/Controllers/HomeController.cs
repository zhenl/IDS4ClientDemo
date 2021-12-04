using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IDS4ClientNet4.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public async Task<ActionResult> WebApi()
        {
            var user = User as ClaimsPrincipal;
            var accessToken = user.FindFirst("access_token").Value;

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await client.GetAsync("http://localhost:5153/WeatherForecast");
            string content;
            if (!response.IsSuccessStatusCode)
            {
                content = await response.Content.ReadAsStringAsync();
                ViewBag.Json = content;
            }
            else
            {
                content = await response.Content.ReadAsStringAsync();
                ViewBag.Json = JArray.Parse(content).ToString();
            }

            return View();
        }

        public ActionResult Logout()
        {
            System.Web.HttpContext.Current.GetOwinContext().Authentication.SignOut();
            return Redirect("/");
        }
    }
}