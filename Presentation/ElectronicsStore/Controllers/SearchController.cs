using Microsoft.AspNetCore.Mvc;
using Store.Web.App;
namespace Store.Web.Controllers
{
    public class SearchController : Controller
    {
        private readonly ComponentService componentService;

        public SearchController(ComponentService componentService)
        {
            this.componentService = componentService;
        }

        public async Task<IActionResult> Index(string query)
        {
            var components = await componentService.GetAllByQueryAsync(query);
            return View("Index", components);
        }
    }
}