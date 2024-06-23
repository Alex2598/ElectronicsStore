using Store.Web.Models;
using Store.Web.Data;
using Microsoft.EntityFrameworkCore;
using Store.Web.CustomPolicy;
using Microsoft.AspNetCore.Authorization;
using Store.Contractors;

using Store.Messages;
using Store.Data.EF;
using Store;
using Store.Web.Contractors;
using Store.YandexKassa;
using Store.Web.App;
using System.Configuration;
namespace Store.Web
{
	public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ApplicationContext>(options =>
                options.UseSqlServer(
                 connectionString
               ));

            var emailOptions = builder.Configuration.GetSection("EmailSender").Get<EmailHelperOptions>() ?? throw new InvalidOperationException("Email Sender options not found."); ;
            builder.Services.AddEmailHelper(emailOptions);


            var requireEmailConfirmed = builder.Configuration.GetValue<bool>("RequireConfirmedEmail");
			builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedEmail = requireEmailConfirmed; //включаем подтверждение адреса электронной почты
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 10;
            })
                .AddRoles<ApplicationRole>() //включаем поддержку ролей
                .AddEntityFrameworkStores<ApplicationContext>();

			builder.Services.ConfigureApplicationCookie(opts => 
            { 
                opts.LoginPath = "/account/signin";
                opts.AccessDeniedPath = "/AccessDanied"; //путь к странице с информацией о запрете доступа
            });

			//настраиваем политику авторизации
			builder.Services.AddTransient<IAuthorizationHandler, OlderThenHandler>();
			builder.Services.AddAuthorization(options => 
            {
                options.AddPolicy("OnlyRussianAdmin", policy => 
                {
                    policy.RequireRole("ADMIN");
                    policy.RequireClaim("Language", "Russian");
                    policy.AddRequirements(new OlderThenPolicy(18)); //пользователь старше 18 лет
                });
            });

            // Add services to the container.
           
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddControllersWithViews();
            builder.Services.AddEfRepositories("Server=ALEX-HONOR;Database=WebApp;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");
            builder.Services.AddSingleton<INotificationService, DebugNotificationService>();
            builder.Services.AddSingleton<IDeliveryService, PostamateDeliveryService>();
            builder.Services.AddSingleton<ComponentService>();
            builder.Services.AddSingleton<IPaymentService, CashPaymentService>();
            builder.Services.AddSingleton<IPaymentService, YandexKassaPaymentService>();
            builder.Services.AddSingleton<IWebContractorService, YandexKassaPaymentService>();
            builder.Services.AddSingleton<OrderService>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (false)
            {
               
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseSession(); 
            
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            app.MapControllerRoute(
                name: "default",
             pattern: "{controller=Home}/{action=Index}/{id?}");

          
            app.Run();
        }
    }
}
