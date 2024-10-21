using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;

namespace Talabat.Repository._Identity
{
    public static class ApplicationIdentityDbContextSeed
    {
        public static async Task SeedUserAsync(UserManager<ApplicationUser> userManager)
        {
            if (!userManager.Users.Any()) // it means if Asp.NetUser in Db is Empty
            {
                var User = new ApplicationUser()
                {
                    DisplayName = "Abdullah Mostafa",
                    Email = "abdallahhassan0119@gmail.com",
                    UserName = "abdallahhassan",
                    PhoneNumber = "01556694608"
                };
                await userManager.CreateAsync(User, "Pa$$w0rd"); 
            }
        }
    }
}
