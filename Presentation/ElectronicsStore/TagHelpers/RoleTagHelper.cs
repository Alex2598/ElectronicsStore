using Store.Web.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Store.Web.TagHelpers
{
    [HtmlTargetElement("td", Attributes = "u-role")]
    public class RoleTagHelper: TagHelper
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;

        [HtmlAttributeName("u-role")]
        public string Role { get; set; }

        public RoleTagHelper(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            List<string?> users = [];
            ApplicationRole role = await roleManager.FindByIdAsync(Role);
            if (role != null)
            {
                users = (await userManager.GetUsersInRoleAsync(role.Name))
                    .Select((user) => user.UserName)
                    .ToList();
            }
            output.Content.SetContent(users.Count == 0 ? "Нет пользователей" : string.Join(", ", users));
        }
    }
} 
    
