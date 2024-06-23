using Microsoft.AspNetCore.Mvc;
namespace Store.Web.Components
{
    public class AccountMenu : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View(HttpContext.User);
        }
    }
}
