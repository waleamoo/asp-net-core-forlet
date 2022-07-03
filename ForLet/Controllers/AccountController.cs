using ForLet.Infrastructure;
using ForLet.Models;
using ForLet.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ForLet.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ForLetDbContext context;
        private readonly UserManager<AppUser> userManager; // manages user 
        private readonly SignInManager<AppUser> signInManager; // manages sign in 
        private IPasswordHasher<AppUser> passwordHasher; // hases the password 
        private readonly IWebHostEnvironment webHostingEnvironment;
        private readonly IAccountRepository _accountRepository;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, 
            IPasswordHasher<AppUser> passwordHasher, ForLetDbContext context, IWebHostEnvironment webHostingEnvironment, IAccountRepository accountRepository)
        {
            this.context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.passwordHasher = passwordHasher;
            this.webHostingEnvironment = webHostingEnvironment;
            _accountRepository = accountRepository;
        }

        // GET: /account/register 
        [AllowAnonymous]
        public IActionResult Register()
        {
            ViewBag.states = new SelectList(context.States.OrderBy(n => n.StateName), "Id", "StateName");
            return View();
        }
        
        // POST: /account/register 
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                string address = $"{user.HouseNumber} {user.StreetName} {user.AreaName}, {context.Lgas.Single(s => s.Id == int.Parse(HttpContext.Request.Form["lga"])).LGAName}, {context.States.Single(s => s.Id == int.Parse(HttpContext.Request.Form["stateId"])).StateName}";
                
                AppUser appUser = new AppUser
                {
                    UserName = user.PhoneNumber,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Address = address,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber
                };

                IdentityResult result = await userManager.CreateAsync(appUser, user.Password);
                if (result.Succeeded)
                {
                    TempData["success"] = "Registration successful. Please login.";

                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.states = new SelectList(context.States.OrderBy(n => n.StateName), "Id", "StateName");

                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(AppUser model)
        {
            ViewBag.states = new SelectList(context.States.OrderBy(n => n.StateName), "Id", "StateName");
            if (ModelState.IsValid)
            {
                AppUser user = await userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    string imageName = "noimage.jpg";
                    ViewBag.Profile = model.ProfileImage.FileName;
                    if (!string.IsNullOrEmpty(model.ProfileImage.FileName))
                    {
                        // check if the user has a previous profile pic, delete the profile pic 
                        string uploadDir = Path.Combine(webHostingEnvironment.WebRootPath, "Media/Users");
                        imageName = Guid.NewGuid().ToString() + "_" + model.ProfileImage.FileName;
                        string filePath = Path.Combine(uploadDir, imageName);
                        FileStream fs = new FileStream(filePath, FileMode.Create);
                        await model.ProfileImage.CopyToAsync(fs);
                        fs.Close();
                    }
                    user.Avatar = imageName.ToString(); // set the profile image location 
                    user.Address = $"{model.HouseNumber} {model.StreetName} {model.AreaName}, {context.Lgas.Single(s => s.Id == model.LgaId).LGAName}, {context.States.Single(s => s.Id == model.StateId).StateName}";
                    user.LastName = model.LastName;
                    user.Email = model.Email;
                    user.PhoneNumber = model.PhoneNumber;
                    user.PasswordHash = passwordHasher.HashPassword(user, model.Password);

                    IdentityResult result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        TempData["success"] = "Update successful";
                        return RedirectToAction("Profile");
                    }
                    else
                        foreach (var errors in result.Errors) ModelState.AddModelError("", errors.Description);
                    return View(model);
                }
                else
                {
                    ModelState.AddModelError("", "Profile Not Found");
                    return View(model);
                }
            }
            else
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var modelError in modelState.Errors)
                    {
                        ModelState.AddModelError("", modelError.ErrorMessage);
                    }
                }
            }
            return View(model);

        }

        public async Task<IActionResult> Profile()
        {
            ViewBag.states = new SelectList(context.States.OrderBy(n => n.StateName), "Id", "StateName");
            var userFind = await userManager.GetUserAsync(User); // get AppUser
            string userId = userFind.Id;
            AppUser appUser = await userManager.FindByIdAsync(userId);
            return View(appUser);

        }

        // GET: /account/login 
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            Login login = new Login
            {
                ReturnUrl = returnUrl
            };
            return View(login);
        }

        // POST: /account/login 
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login login)
        {
            // get the id of the user using the PhoneNumber
            //var userFind = await userManager.GetUserAsync(User);
            //var userFind = await userManager.GetUserAsync(User);
            //string id = null;
            //id = context.Users.FirstOrDefault(x => x.PhoneNumber == login.PhoneNumber).Id;
            // context.MAIN_GLOBAL.Where(x => x.ITEM_NM == optionStr).Select(x => x.ITEM_TXT).FirstOrDefault();
            //string nullRef = null;
            string id = context.Users.Where(x => x.PhoneNumber == login.PhoneNumber).Select(x => x.Id).FirstOrDefault();
            if (!(id == null))
            {
                if (ModelState.IsValid)
                {
                    AppUser appUser = await userManager.FindByIdAsync(id); // gets the user by ID rather than email
                    if (appUser != null)
                    {
                        Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.PasswordSignInAsync(appUser, login.Password, false, false);
                        if (result.Succeeded)
                        {
                            return Redirect(login.ReturnUrl ?? "/account/profile");
                        }
                        ModelState.AddModelError("", $"If an account exist for {login.PhoneNumber}, credentials does not match. You can also try logging in with your phone number as password if you never registered. Try again.");
                    }
                    ModelState.AddModelError("", "Login failed. Wrong credentials.");
                }
            }
            else
            {
                TempData["error"] = "Invalid login.";
                return View(login);
            }
            return View(login);
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return Redirect("/");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rental(RentalViewModel rentalVM)
        {
            var rental = new Rental();
            rental.Title = context.PropertyTypes.Single(p => p.Id == rentalVM.PropertyTypeId).PropertyName + " available for rent";
            rental.PropertyTypeId = rentalVM.PropertyTypeId;
            // get the features and add to the description 
            // rental.Features 
            rental.RentalDescription = $"{context.PropertyTypes.Single(p => p.Id == rentalVM.PropertyTypeId).PropertyName} is available for rent at { rentalVM.AreaName}";
            rental.RentalAddress = $"{rentalVM.HouseNumber} {rentalVM.StreetName} {rentalVM.AreaName}, {context.Lgas.Single(s => s.Id == rentalVM.LgaId).LGAName}, {context.States.Single(s => s.Id == rentalVM.StateId).StateName}";
            rental.NoOfApartment = rentalVM.NoOfApartment;
            rental.Rent = rentalVM.Rent;
            rental.Agreement = rentalVM.Agreement.HasValue ? rentalVM.Agreement.Value : 0;
            rental.Damages = rentalVM.Damages.HasValue ? rentalVM.Damages.Value : 0;
            rental.DatePosted = DateTime.Now;
            rental.StateId = rentalVM.StateId;
            rental.LgaId = rentalVM.LgaId;
            rental.ClientId = rentalVM.UserId;

            if (ModelState.IsValid)
            {
                context.Rentals.Add(rental);
                rental.slug = $"{context.PropertyTypes.Single(p => p.Id == rentalVM.PropertyTypeId).PropertyName}-for-rent-{context.Lgas.Single(s => s.Id == rentalVM.LgaId).LGAName}-{context.States.Single(s => s.Id == rentalVM.StateId).StateName}-{rental.Id}".Replace(" ", "-").ToLower();
                if (rentalVM.GalleryFiles != null)
                {
                    // upload the files and create a reference 
                    string folder = "Media/Rentals";
                    rentalVM.image = new List<Image>();
                    if(rentalVM.GalleryFiles.Count > 3) 
                    {
                        TempData["error"] = "Files cannot be more than 3";
                        return RedirectToAction("Rental");
                    }
                    else
                    {
                        foreach (var file in rentalVM.GalleryFiles)
                        {
                            var image = new Image()
                            {
                                RentalId = rental.Id,
                                ImageLocation = await UploadImage(folder, file)
                            };
                            rentalVM.image.Add(image);
                        }
                    }
                }

                // saves the files to the database 
                rental.image = new List<Image>(); // creates an empty list 
                foreach (var file in rentalVM.image)
                {
                    rental.image.Add(new Image()
                    {
                        RentalId = file.RentalId,
                        ImageLocation = file.ImageLocation
                    });
                }
                await context.SaveChangesAsync();
                TempData["success"] = "Rental ad posted successfully.";
                return RedirectToAction("Rental");

            }
            else
            {
                var modelErrors = new List<string>();
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var modelError in modelState.Errors)
                    {
                        ModelState.AddModelError("", modelError.ErrorMessage);
                    }
                }
            }
            return View();
        }

        private async Task<string> UploadImage(string folderPath, IFormFile file)
        {
            string uploadDir = Path.Combine(webHostingEnvironment.WebRootPath, folderPath);
            string imageName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadDir, imageName);
            await file.CopyToAsync(new FileStream(filePath, FileMode.Create));
            return imageName;
        }


        public async Task<IActionResult> Rental()
        {
            var userFind = await userManager.GetUserAsync(User); // get AppUser
            ViewBag.id = userFind.Id;
            ViewBag.phoneNumber = userFind.PhoneNumber;
            ViewBag.email = userFind.Email;
            ViewBag.fullName = $"{userFind.FirstName} {userFind.LastName}";
            ViewBag.states = new SelectList(context.States.OrderBy(n => n.StateName), "Id", "StateName");
            ViewBag.propertyTypes = new SelectList(context.PropertyTypes.Where(n => n.PropertyName != "Shortlet Rentals"), "Id", "PropertyName");
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShortLetRental(RentalViewModel rentalVM)
        {
            
            if (ModelState.IsValid)
            {
                var rental = new Rental();
                rental.Title = context.PropertyTypes.Single(p => p.Id == rentalVM.PropertyTypeId).PropertyName + " flat available for rent";
                rental.PropertyTypeId = rentalVM.PropertyTypeId;
                // get the features and add to the description 
                // rental.Features 
                rental.RentalDescription = $"{context.PropertyTypes.Single(p => p.Id == rentalVM.PropertyTypeId).PropertyName} is available for rent at { rentalVM.AreaName}";
                rental.RentalAddress = $"{rentalVM.HouseNumber} {rentalVM.StreetName} {rentalVM.AreaName}, {context.Lgas.Single(s => s.Id == rentalVM.LgaId).LGAName}, {context.States.Single(s => s.Id == rentalVM.StateId).StateName}";
                rental.NoOfApartment = rentalVM.NoOfApartment;
                rental.Rent = rentalVM.Rent;
                rental.Agreement = rentalVM.Agreement.HasValue ? rentalVM.Agreement.Value : 0;
                rental.Damages = rentalVM.Damages.HasValue ? rentalVM.Damages.Value : 0;
                rental.DatePosted = DateTime.Now;
                rental.StateId = rentalVM.StateId;
                rental.LgaId = rentalVM.LgaId;
                rental.ClientId = rentalVM.UserId;
                context.Rentals.Add(rental);
                if (rentalVM.GalleryFiles != null)
                {
                    string folder = "Media/Rentals";
                    rentalVM.image = new List<Image>();
                    foreach (var file in rentalVM.GalleryFiles)
                    {
                        var image = new Image()
                        {
                            RentalId = rental.Id,
                            ImageLocation = await UploadImage(folder, file)
                        };
                        rentalVM.image.Add(image);
                    }
                }

                await context.SaveChangesAsync();
                TempData["success"] = "Rental ad posted successfully.";
                return RedirectToAction("Rental");
            }
            else
            {
                var modelErrors = new List<string>();
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var modelError in modelState.Errors)
                    {
                        ModelState.AddModelError("", modelError.ErrorMessage);
                    }
                }
            }


            return View();
        }

        public IActionResult ShortLetRental()
        {
            return View();
        }
            
        public IActionResult Messages()
        {
            return View();
        }

        public async Task<IActionResult> Rentals()
        {
            var userFind = await userManager.GetUserAsync(User); // get AppUser
            string userId = userFind.Id;

            var rentals = context.Rentals.Include(x => x.image).Where(x => x.ClientId == userId);
            List<Rental> rentalList = await rentals.ToListAsync();
            return View(rentalList);
        }

        public IActionResult RentalDetails(int id)
        {
            var rental = context.Rentals.Include(i => i.image).Where(x => x.Id == id).First();
            return View(rental);
        }
        
        public async Task<IActionResult> DeleteRental(int id)
        {
            var rental = await context.Rentals.Include(x => x.image).FirstAsync(x => x.Id == id);
            if (rental == null)
            {
                TempData["error"] = "Rental not found";
            }
            else
            {
                if (rental.image.Count() > 0)
                {
                    foreach (var image in rental.image)
                    {
                        string uploadDir = Path.Combine(webHostingEnvironment.WebRootPath, "Media/Rentals");
                        string oldImagePath = Path.Combine(uploadDir, image.ImageLocation);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                }
                context.Rentals.Remove(rental);
                await context.SaveChangesAsync();
                TempData["success"] = "Rental ad deleted";
            }
            return RedirectToAction("Rentals");
        }

        public IActionResult DeleteImage(string name)
        {
            FileInfo file = new FileInfo(Path.Combine(webHostingEnvironment.WebRootPath, "Media\\Rentals\\", name));
            if (file.Exists)
            {
                file.Delete();
            }


            const string query = "DELETE FROM [dbo].[Images] WHERE [ImageLocation]={0}";
            context.Database.ExecuteSqlRaw(query, name);
            //ForLet.Models.Image imageContext = context.Images.Where(x => x.ImageLocation == name);
            //context.Images.Remove(imageContext);
            //context.SaveChanges();
            TempData["success"] = "Image deleted";
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [AllowAnonymous, HttpGet("forgot-password")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [AllowAnonymous, HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _accountRepository.GetUserByEmailAsync(model.Email);
                if (user != null) // user exist 
                {
                    await _accountRepository.GenerateForgotPasswordTokenAsync(user);
                }
                ModelState.Clear();
                model.EmailSent = true;
            }
            return View(model);
        }

        [AllowAnonymous, HttpGet("reset-password")]
        public IActionResult ResetPassword(string uid, string token)
        {
            ResetPasswordModel resetPasswordModel = new ResetPasswordModel
            {
                Token = token,
                UserId = uid
            };
            return View(resetPasswordModel);
        }

        [AllowAnonymous, HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                model.Token = model.Token.Replace(' ', '+');
                var result = await _accountRepository.ResetPasswordAsync(model);
                if (result.Succeeded)
                {
                    ModelState.Clear(); 
                    model.IsSuccess = true;
                    return View(model);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }



    }
}
