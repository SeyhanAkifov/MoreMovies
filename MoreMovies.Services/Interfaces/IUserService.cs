using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMovies.Services.Interfaces
{
    public interface IUserService
    {
        public Task<ICollection<IdentityUser>> GetAll();
        public Task Become(string userId);
        public Task<IdentityUser> GetUser(string userName);
    }
}
