using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using WebExtract.Models;

namespace WebExtract.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _env;

        public HomeController(IHostingEnvironment env)
        {
            _env = env;
        }

        private List<string> GetRoles(string sampleHtml)
        {
            //https://github.com/trenoncourt/HtmlAgilityPack.CssSelectors.NetCore
            var doc = new HtmlAgilityPack.HtmlDocument();

            string htmlContent = System.IO.File.ReadAllText(sampleHtml);
            doc.LoadHtml(htmlContent);
            
            //doc.Load(new FileStream(sampleHtml, FileMode.Open));

            IList<HtmlNode> nodes = doc.QuerySelectorAll("[asp-authorize]");
            
            if (nodes == null || !nodes.Any(x => x.Attributes.Any(y => y.Name == "asp-roles"))) return new List<string>();

            var authorizeNodes = nodes.Where(x => x.Attributes.Any(y => y.Name == "asp-roles"));
            var authorizeAttrs = authorizeNodes.SelectMany(x => x.Attributes.Where(y => y.Name == "asp-roles"));

            return authorizeAttrs.Select(x => x.Value).ToList();
        }

        public IActionResult Index()
        {
            //https://mariusschulz.com/blog/getting-the-web-root-path-and-the-content-root-path-in-asp-net-core
            var webRootPath = _env.WebRootPath;
            var sampleFilePath = webRootPath + "\\sample2.html";
            var roles1 = GetRoles(sampleFilePath);

            //--
            var contentRootPath = _env.ContentRootPath;
            var viewsPath = contentRootPath + "\\Views";
            string[] filePaths = System.IO.Directory.GetFiles(viewsPath, "*.cshtml", SearchOption.AllDirectories);
            var roles2 = new List<string>();

            foreach(var path in filePaths)
            {
                var roles = GetRoles(path);

                roles2.AddRange(roles);
            }
                       
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
