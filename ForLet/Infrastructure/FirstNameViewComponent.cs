using ForLet.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ForLet.Infrastructure
{
    public class FirstNameViewComponent : ViewComponent
    {

        private readonly ForLetDbContext context;
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        public FirstNameViewComponent(ForLetDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            this.context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userFind = await userManager.GetUserAsync(HttpContext.User); // get AppUser
            var firstName = userFind.FirstName;
            ViewBag.UserName = firstName;
            return View();
        }
    }
}
