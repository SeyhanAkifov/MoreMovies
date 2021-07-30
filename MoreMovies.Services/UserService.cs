using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MoreMovies.Data;
using MoreMovies.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoreMovies.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext db;

        public UserService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<ICollection<IdentityUser>> GetAll()
        {
            var users =  await this.db.Users.ToArrayAsync();

            return users;
        }
    }
}
