using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Store.Web.Models;
using System.Security.Claims;
namespace Store.Web.Areas.Account.Controllers
{
	[Authorize]
	public class ClaimsController : Controller
	{
		[Route("{area}/{controller}")]
		public IActionResult Index()
		{
			return View(User?.Claims);
		}

		[Route("{area}/{controller}/{action}")]
		[HttpGet]
		public IActionResult Create()
		{
			return View();
		}

		[Route("{area}/{controller}/{action}")]
		[HttpPost]
		public async Task<IActionResult> Create([FromServices]UserManager<ApplicationUser> users, string claimType, string claimValue)
		{
			var user = await users.GetUserAsync(HttpContext.User);
			Claim claim = new(claimType, claimValue, ClaimValueTypes.String);
			IdentityResult result = await users.AddClaimAsync(user, claim);

			if (result.Succeeded)
				return LocalRedirect("/account/claims");
			else
				return BadRequest(result);
		}

		[HttpPost]
		[Route("{area}/{controller}/{action}")]
		public async Task<IActionResult> Delete([FromServices] UserManager<ApplicationUser> users, string claimValues)
		{
			var user = await users.GetUserAsync(HttpContext.User);

			string[] claimValuesArray = claimValues.Split(";");
			string claimType = claimValuesArray[0];
			string claimValue = claimValuesArray[1]; 
			string claimIssuer = claimValuesArray[2];

			Claim claim = User.Claims.Where(x => x.Type == claimType && x.Value == claimValue && x.Issuer == claimIssuer).FirstOrDefault();

			IdentityResult result = await users.RemoveClaimAsync(user, claim);

			if (result.Succeeded)
				return LocalRedirect("/account/claims");
			else
				return BadRequest(result);
		}
	}
}
