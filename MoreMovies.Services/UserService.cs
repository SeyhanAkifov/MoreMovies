using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MoreMovies.Data;
using MoreMovies.Services.Interfaces;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MoreMovies.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext db;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<IdentityUser> userManager;

        public UserService(ApplicationDbContext db , UserManager<IdentityUser> userManager,
           RoleManager<IdentityRole> roleManager)
        {
            this.db = db;
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

       

        public async Task<ICollection<IdentityUser>> GetAll()
        {
            var users =  await this.db.Users.ToArrayAsync();

            return users;
        }

        public async Task Become(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            IdentityRole role = new()
            {
                Name = "Manager"
            };
            var result = await roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Manager");
            }
            
        }

        public async Task<IdentityUser> GetUser(string userName)
        {
            var user = await userManager.FindByEmailAsync(userName);

            return user;
        }


    }
}
