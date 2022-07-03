using ForLet.Infrastructure;
using ForLet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ForLet.Areas.Admin.Controllers
{
    //[Authorize(Roles = "admin")]
    [Area("Admin")]
    public class RolesController : Controller
    {

        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<AppUser> userManager;
        private readonly ForLetDbContext context;

        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, ForLetDbContext context)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.context = context;
        }

        public IActionResult Index()
        {
            return View(roleManager.Roles);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([MinLength(2), Required] string name)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await roleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                {
                    TempData["success"] = "Role created successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (IdentityError er in result.Errors) ModelState.AddModelError("", er.Description);
                }
            }
            ModelState.AddModelError("", "Minimum length is");
            return View();
        }

        public async Task<IActionResult> Edit(string id)
        {
            IdentityRole role = await roleManager.FindByIdAsync(id);
            List<AppUser> members = new List<AppUser>();
            List<AppUser> nonMembers = new List<AppUser>();
            foreach (AppUser user in userManager.Users)
            {
                var list = await userManager.IsInRoleAsync(user, role.Name) ? members : nonMembers;
                list.Add(user);
            }

            return View(new RoleEdit { Role = role, Members = members, NonMembers = nonMembers });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RoleEdit roleEdit)
        {
            IdentityResult result;
            foreach (string userId in roleEdit.AddIds ?? new string[] { })
            {
                AppUser user = await userManager.FindByIdAsync(userId);
                result = await userManager.AddToRoleAsync(user, roleEdit.RoleName);
            }
            foreach (string userId in roleEdit.DeleteIds ?? new string[] { })
            {
                AppUser user = await userManager.FindByIdAsync(userId);
                result = await userManager.RemoveFromRoleAsync(user, roleEdit.RoleName);
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        public async Task<IActionResult> Delete(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                TempData["error"] = "Role not found";
                return Redirect(Request.Headers["Referer"].ToString());
            }
            else
            {
                var result = await roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    TempData["success"] = "Role deleted successfully";
                    return Redirect(Request.Headers["Referer"].ToString());
                }
                TempData["error"] = "Error occured deleting row";
                return Redirect(Request.Headers["Referer"].ToString());

            }
        }


    }
}
