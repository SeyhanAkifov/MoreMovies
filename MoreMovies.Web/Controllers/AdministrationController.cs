using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MoreMovie.Web.Models;
using MoreMovies.Data;
using MoreMovies.Models;
using MoreMovies.Services.Interfaces;
using MoreMovies.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoreMovie.Web.Controllers
{
    [Authorize]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<IdentityUser> userManager;
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly INewsService newsService;
        private readonly IComingSoonService comingSoonService;

        public AdministrationController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context, IMapper mapper, INewsService newsService, IComingSoonService comingSoonService)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.context = context;
            this.mapper = mapper;
            this.newsService = newsService;
            this.comingSoonService = comingSoonService;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        public async Task<IActionResult> EditComingSoon()
        {
            var comingSoon = await this.comingSoonService.GetAll();
            var comingSoonResult = mapper.Map<ICollection<ComingSoon>, ICollection<ComingSoonViewModel>>(comingSoon);


            return this.View(comingSoonResult);
        }

        public async Task<IActionResult> DeleteComingSoon(int id)
        {
            await this.comingSoonService.Delete(id);


            return this.RedirectToAction("EditComingSoon");
        }

        public async Task<IActionResult> EditNews()
        {
            var news = await this.newsService.GetAllNews();
            var newsResult = mapper.Map<ICollection<News>, ICollection<NewsViewModel>>(news);

            return this.View(newsResult);
        }

        public async Task<IActionResult> DeleteNews(int id)
        {
            await this.newsService.Delete(id);


            return this.RedirectToAction("EditNews");
        }


        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id: {roleId} cannot be found";
                return View("NoteFound");
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);

                IdentityResult result = null;

                if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                    {
                        continue;
                    }
                    else
                    {
                        return RedirectToAction("EditRoles", new { Id = roleId });
                    }
                }
            }

            return RedirectToAction("EditRoles", new { Id = roleId });
        }


        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;

            var role = await roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id: {roleId} cannot be found";
                return View("NoteFound");
            }

            var model = new List<UserRoleViewModel>();

            foreach (var user in userManager.Users)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName

                };

                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }

                model.Add(userRoleViewModel);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditRoles(string id)
        {
            var role = await roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id: {id} cannot be found";
                return View("Error");
            }

            var model = new EditRoleViewModel
            {
                RoleId = role.Id,
                RoleName = role.Name
            };

            foreach (var user in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRoles(EditRoleViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.RoleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id: {model.RoleId} cannot be found";
                return View("Error");
            }
            else
            {
                role.Name = model.RoleName;
                var result = await roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }
            }
            
            return View(model);
        }

        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole role = new IdentityRole
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

        [HttpGet]
        public IActionResult CreateRole()
        {
            return this.View();
        }

        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);

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



        public IActionResult All()
        {
            var users = this.context.Users
                .ProjectTo<MyProfileViewModel>(mapper.ConfigurationProvider)
                .ToList();

            return this.View(users);
        }


    }
}
