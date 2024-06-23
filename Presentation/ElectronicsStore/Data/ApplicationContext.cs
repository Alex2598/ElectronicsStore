using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Store.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Store.Web.Data
{
    public class ApplicationContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            //Database.Migrate();
        }
    }
}
