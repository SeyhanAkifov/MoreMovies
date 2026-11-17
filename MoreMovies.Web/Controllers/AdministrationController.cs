using MoreMovies.Web.MappingConfiguration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MoreMovies.Web.Models.Administration;
using MoreMovies.Models;
using MoreMovies.Services.Dto.Output;
using MoreMovies.Services.Interfaces;
using MoreMovies.Web.Models;
using MoreMovies.Web.Models.News;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;

namespace MoreMovies.Web.Controllers
{
    [Authorize(Roles = RoleNames.Admin)]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<IdentityUser> userManager;
        private readonly INewsService newsService;
        private readonly IComingSoonService comingSoonService;
        private readonly IUserService userService;

        public AdministrationController(
            UserManager<IdentityUser> userManager, 
            RoleManager<IdentityRole> roleManager, 
            INewsService newsService,
            IComingSoonService comingSoonService, 
            IUserService userService)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.newsService = newsService;
            this.comingSoonService = comingSoonService;
            this.userService = userService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EditComingSoon()
        {
            var comingSoon = await this.comingSoonService.GetAll();
            var comingSoonResult = (comingSoon ?? System.Array.Empty<ComingSoonOutputDto>()).Select(ViewModelMappings.ToViewModel).ToList();
            
            return this.View(comingSoonResult);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComingSoon(int id)
        {
            await this.comingSoonService.Delete(id);
            
            return this.RedirectToAction("EditComingSoon");
        }

        [HttpGet]
        public async Task<IActionResult> EditNews()
        {
            var news = await this.newsService.GetAllNews();
            var newsResult = (news ?? System.Array.Empty<NewsOutputDto>()).Select(ViewModelMappings.ToViewModel).ToList();

            return this.View(newsResult);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteNews(int id)
        {
            await this.newsService.Delete(id);
            
            return this.RedirectToAction("EditNews");
        }


        /// <summary>Apply changes to role membership.</summary>
        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId ?? "");
            if (role is null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (role.Name == RoleNames.Admin && model.Any(entry => entry.UserId == currentUserId && !entry.IsSelected))
            {
                return BadRequest("You cannot remove your own administrator access.");
            }

            foreach (var entry in model)
            {
                var user = await userManager.FindByIdAsync(entry.UserId ?? "");
                if (user is null || entry.IsSelected == await userManager.IsInRoleAsync(user, role.Name))
                {
                    continue;
                }

                var result = entry.IsSelected
                    ? await userManager.AddToRoleAsync(user, role.Name)
                    : await userManager.RemoveFromRoleAsync(user, role.Name);
                if (!result.Succeeded)
                {
                    return BadRequest("A role assignment could not be updated.");
                }
            }

            return RedirectToAction(nameof(EditRoles), new { id = roleId });
        }

        /// <summary>Show users and their current role membership.</summary>
        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId ?? "");
            if (role is null)
            {
                return NotFound();
            }

            ViewBag.RoleId = role.Id;
            ViewBag.ProtectedUserId = role.Name == RoleNames.Admin
                ? User.FindFirstValue(ClaimTypes.NameIdentifier)
                : null;
            var members = (await userManager.GetUsersInRoleAsync(role.Name)).Select(user => user.Id).ToHashSet();
            var model = userManager.Users.OrderBy(user => user.UserName).AsEnumerable()
                .Select(user => new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    IsSelected = members.Contains(user.Id)
                }).ToList();

            return View(model);
        }

        /// <summary>Show the selected role and its members.</summary>
        [HttpGet]
        public async Task<IActionResult> EditRoles(string id)
        {
            var role = await roleManager.FindByIdAsync(id ?? "");
            return role is null ? NotFound() : View(await BuildRoleViewAsync(role));
        }

        /// <summary>Rename a custom role after validating its new name.</summary>
        [HttpPost]
        public async Task<IActionResult> EditRoles(EditRoleViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.RoleId ?? "");
            if (role is null)
            {
                return NotFound();
            }

            if (RoleNames.IsBuiltIn(role.Name))
            {
                return BadRequest("Built-in roles cannot be renamed.");
            }

            if (ModelState.IsValid)
            {
                role.Name = model.RoleName;
                var result = await roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(ListRoles));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(nameof(model.RoleName), error.Description);
                }
            }

            return View(await BuildRoleViewAsync(role));
        }

        /// <summary>Build the role editor with its current members.</summary>
        private async Task<EditRoleViewModel> BuildRoleViewAsync(IdentityRole role)
        {
            var users = await userManager.GetUsersInRoleAsync(role.Name);
            return new EditRoleViewModel
            {
                RoleId = role.Id,
                RoleName = role.Name,
                NameLocked = RoleNames.IsBuiltIn(role.Name),
                Users = users.Select(user => user.UserName).OrderBy(name => name).ToList()
            };
        }

        /// <summary>Show roles in alphabetical order.</summary>
        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = roleManager.Roles.OrderBy(role => role.Name).ToList();
            return View(roles);
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole role = new()
                {
                    Name = model.RoleName
                };
                var result = await roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("index", "home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(" ", error.Description);
                }
            }

            return this.View(model);
        }

        /// <summary>Delete another user from administration.</summary>
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (id == User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return BadRequest("You cannot delete your own account from administration.");
            }

            var user = await userManager.FindByIdAsync(id ?? "");

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id: {id} cannot be found";
            }
            else
            {
                var result = await userManager.DeleteAsync(user);
                
                if (result.Succeeded)
                {
                    return RedirectToAction("All", "Administration");
                }
            }

            return RedirectToAction("All", "Administration");
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var users = await userService.GetAll();
            var usersResult = (users ?? System.Array.Empty<IdentityUser>()).Select(ViewModelMappings.ToViewModel).ToList()
                .ToList();

            return this.View(usersResult);
        }
    }
}
