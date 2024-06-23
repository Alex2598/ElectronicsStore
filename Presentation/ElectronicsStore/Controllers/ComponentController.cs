using Microsoft.AspNetCore.Mvc;
using Store.Web.App;
namespace Store.Web.Controllers
{
    public class ComponentController : Controller
    {
        private readonly ComponentService componentService;
        public ComponentController(ComponentService componentService)
        {
            this.componentService = componentService;
        }

        public async Task<IActionResult> Index(int id)
        {
            var model = await componentService.GetByIdAsync(id);

            return View(model);
        }
    }
}