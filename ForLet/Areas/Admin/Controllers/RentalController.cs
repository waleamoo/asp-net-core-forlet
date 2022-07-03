using ForLet.Infrastructure;
using ForLet.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForLet.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RentalController : Controller
    {
        private readonly SignInManager<AppUser> signInManager;
        private readonly ForLetDbContext context;
        public RentalController(ForLetDbContext context, SignInManager<AppUser> signInManager)
        {
            this.context = context;
            this.signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            // or IQueryable<Rental> rentals
            var rentals = context.Rentals.Include(u => u.Client).Include(s => s.state).Include(p => p.propertyType).Include(l => l.lga).OrderByDescending(r => r.DatePosted);
            //.Include(u => u.User)
            //.OrderByDescending(x => x.DatePosted);
            List<Rental> rentalList = await rentals.ToListAsync();
            return View(rentalList);
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return Redirect("/");
        }


    }
}
