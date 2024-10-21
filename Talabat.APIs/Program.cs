using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Runtime.InteropServices;
using System.Text;
using Talabat.APIs.Extensions;
using Talabat.APIs.Middlewares;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services.Contract.AuthService;
using Talabat.Repository._Identity;
using Talabat.Repository.Data;
using Talabat.Repository.Data.DataSeed;
using Talabat.Repository.Generic_Repository;
using Talabat.Service.AuthService;

namespace Talabat.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var webApplicationBuilder = WebApplication.CreateBuilder(args);


            #region Configure-Service
            // Add services to the container.

            webApplicationBuilder.Services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;    
            });

            webApplicationBuilder.Services.AddSwaggerServices();    

            webApplicationBuilder.Services.AddDbContext<StoreContext>(options =>
            {

                options.UseSqlServer(webApplicationBuilder.Configuration.GetConnectionString("DefaultConnection"));

            });


            webApplicationBuilder.Services.AddApplicationServices();

            webApplicationBuilder.Services.AddSingleton<IConnectionMultiplexer>((services) =>
            {
                var connection = webApplicationBuilder.Configuration.GetConnectionString("redis");
                return ConnectionMultiplexer.Connect(connection);
            });

            webApplicationBuilder.Services.AddScoped(typeof(IBasketRepository), typeof(BasketRepository));

            webApplicationBuilder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
            {
                options.UseSqlServer(webApplicationBuilder.Configuration.GetConnectionString("IdentityConnection"));
            });

            webApplicationBuilder.Services.AddSwaggerDocumentation();

            webApplicationBuilder.Services.AddIdentityServices();
            //webApplicationBuilder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            //    .AddEntityFrameworkStores<ApplicationIdentityDbContext>();


            webApplicationBuilder.Services.AddAuthServices(webApplicationBuilder.Configuration);

            webApplicationBuilder.Services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", policyOptions =>
                {
                    policyOptions.AllowAnyHeader().AllowAnyMethod().WithOrigins(webApplicationBuilder.Configuration["FrontBaseUrl"]);
                });
            });


            #endregion

            var app = webApplicationBuilder.Build();

            #region Apply All Pending Migrations [Update-Database] and Data Seeding

            using var Scope = app.Services.CreateScope();

            var services = Scope.ServiceProvider;

            var dbContext = services.GetRequiredService<StoreContext>(); // Ask CLR For Creating an Object From DbContext to Update the Database.
            var _identityDbContext = services.GetRequiredService<ApplicationIdentityDbContext>(); // Ask CLR For Creating an Object From DbContext to Update the Database.
            var LoggerFactory = services.GetRequiredService<ILoggerFactory>();
            

            var logger = LoggerFactory.CreateLogger<Program>();

            try
            {
                await dbContext.Database.MigrateAsync(); // Update-Database
                await StoreContextSeed.SeedAsync(dbContext); // Data Seeding

                await _identityDbContext.Database.MigrateAsync(); // Update-Database

                var _userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                await ApplicationIdentityDbContextSeed.SeedUserAsync(_userManager); // Data Seeding
            }
            catch (Exception ex)
            {

                 logger.LogError(ex, "An Error Occurred During Applying The Migration");
            }

            #endregion


            #region Configure Kesterl Middelwares 
            // Configure the HTTP request pipeline.

            app.UseMiddleware<ExceptionMiddleWare>();

            app.UseStatusCodePagesWithReExecute("/errors/{0}");

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseCors("MyPolicy");

            app.MapControllers(); 

            app.UseAuthentication();
            
            app.UseAuthorization();


            #endregion


            app.Run();
        }
    }
}
