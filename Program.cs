using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MVCCRUD.Data;
using MVCCRUD.Models.Domain;
using MVCCRUD.Models.Service;

namespace MVCCRUD
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<Mydbcontext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddIdentity<ApplicationUser,IdentityRole>().AddEntityFrameworkStores<Mydbcontext>().AddDefaultTokenProviders();
            builder.Services.ConfigureApplicationCookie(op => op.LoginPath = "/UserAuthentcation/Login");
            builder.Services.AddScoped<IUserAuthorizationService, UserAuthorizationService>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=UserAuthorization}/{action=Login}/{id?}");

            app.Run();
        }
    }
}