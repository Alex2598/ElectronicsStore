using Microsoft.AspNetCore.Identity;

using System.Security.Claims;

namespace Store.Web.Models
{
    public class ApplicationUser: IdentityUser<int>
    {
        public DateOnly Birthday { get; set; }
        public string Organization {  get; set; }
        public string Departmenet {  get; set; }

        
    }
}
