using Store.Web.Areas.Account.Models;
using Store.Web.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Store.Web.Areas.Account.Controllers
{
	[Area("Account")]
	public class RoleController : Controller
	{
		RoleManager<ApplicationRole> _manager;
		UserManager<ApplicationUser> _users;

		public RoleController(RoleManager<ApplicationRole> manager, UserManager<ApplicationUser> users) 
		{ 
			_manager = manager;
			_users = users;
		}

		[Route("{area}/{action}")]
		[HttpGet]
		public IActionResult Role()
		{
			return View(_manager.Roles);
		}

		[Route("{area}/{action}")]
		[HttpPost]
		public IActionResult Role([FromForm]string roleName)
		{
			_manager.CreateAsync(new ApplicationRole() 
			{ 
				Name = roleName 
			});
			return RedirectToAction("Role");
		}

        [Route("{area}/{action}/{id}")]
		[HttpGet]
        public async Task<IActionResult> Update(string id) 
		{
            ApplicationRole role = await _manager.FindByIdAsync(id); //ищем роль по её ID
            List<ApplicationUser> members = new List<ApplicationUser>(); 
            List<ApplicationUser> nonMembers = new List<ApplicationUser>();
            foreach (ApplicationUser user in _users.Users) //перебираем всех пользователей
            {
                //выбираем список в который необходимо поместить пользователя
                var list = await _users.IsInRoleAsync(user, role.Name) ? members : nonMembers;
                //добавляем пользователя в список
                list.Add(user);
            }
            return View(new RoleEdit
            {
                Role = role,
                Members = members,
                NonMembers = nonMembers
            });
		}

        [Route("{area}/{action}/{id}")]
        [HttpPost]
        public async Task<IActionResult> Update([FromForm] RoleModification model)
        {
            var role = await _manager.FindByIdAsync(model.RoleId);
            role.Name = model.RoleName;
            await _manager.UpdateAsync(role);

            IdentityResult result;
            if (ModelState.IsValid)
            {
                foreach (string userId in model.AddIds ?? new string[] { })
                {
                    ApplicationUser user = await _users.FindByIdAsync(userId);
                    if (user != null)
                    {
                        result = await _users.AddToRoleAsync(user, model.RoleName);
                        if (!result.Succeeded)
                           return BadRequest(result);
                    }
                }
                foreach (string userId in model.DeleteIds ?? new string[] { })
                {
                    ApplicationUser user = await _users.FindByIdAsync(userId);
                    if (user != null)
                    {
                        result = await _users.RemoveFromRoleAsync(user, model.RoleName);
                        if (!result.Succeeded)
                            return BadRequest(result);
                    }
                }
            }

            return RedirectToAction("Role");
        }

		[Route("{area}/{action}/{id}")]
		[HttpPost]
		public async Task<IActionResult> Delete(string id)
		{
			var role = await _manager.FindByIdAsync(id);
			if (role != null)
			   await _manager.DeleteAsync(role);
			return RedirectToAction("Role");
        }

		
	}
}
