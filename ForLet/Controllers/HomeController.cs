using ForLet.Infrastructure;
using ForLet.Models;
using ForLet.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ForLet.Controllers
{
    public class HomeController : Controller
    {
        private readonly ForLetDbContext context;
        private readonly UserManager<AppUser> userManager;
        private readonly IWebHostEnvironment webHostingEnvironment;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        public HomeController(ForLetDbContext context, UserManager<AppUser> userManager, IWebHostEnvironment webHostingEnvironment, IUserService userService, IEmailService emailService)
        {
            this.context = context;
            this.userManager = userManager;
            this.webHostingEnvironment = webHostingEnvironment;
            _userService = userService;
            _emailService = emailService;
        }
        // GET: Index
        public IActionResult Index()
        {
            // Load the list drop down 
            ViewBag.states = new SelectList(context.States.OrderBy(n => n.StateName), "Id", "StateName");
            ViewBag.propertyTypes = new SelectList(context.PropertyTypes.OrderByDescending(n => n.Id), "Id", "PropertyName");

            // send test email - change the method to async
            //UserEmailOptions options = new UserEmailOptions
            //{
            //    ToEmails = new List<string>() { "test@gmail.com" },
            //    PlaceHolders = new List<KeyValuePair<string, string>>()
            //    {
            //        new KeyValuePair<string, string>("{{ UserName }}", "Olawale")
            //    }
            //};
            //await _emailService.SendTestEmail(options);
            // end of email 

            return View();
        }

        public string SendTestEmail()
        {
            var client = new System.Net.Mail.SmtpClient("smtp.mailtrap.io", 2525)
            {
                Credentials = new System.Net.NetworkCredential("291da320f8c09c", "f504cc3daa0f0f"),
                EnableSsl = true
            };
            client.Send("from@example.com", "to@example.com", "Hello world", "testbody");
            return "Sent";
        }

        [HttpGet("faq")]
        public IActionResult FAQ() 
        {
            return View();
        }
        
        [HttpGet("contact")]
        public IActionResult Contact() 
        {
            return View();
        }

        [HttpGet]
        public string GetLga([FromQuery] string stateId, string dependent)
        {
            string output = "";
            int stateIdNumber = int.Parse(stateId);
            var lgas = context.Lgas.Where(x => x.StateId == stateIdNumber).ToList();
            output += "<option value=\"0\">Any L.G.A </option>";
            foreach (var lga in lgas)
            {
                output += "<option value=\"" + lga.Id + "\">" + lga.LGAName + "</option>";
            }
            return output;
        }
        
        [HttpGet]
        public JsonResult GetUser([FromQuery] string userNumber)
        {
            
            var user = context.Users.First(x => x.PhoneNumber == userNumber);
            if (!(user == null))
            {
                //response.fullName = $"{user.LastName} {user.FirstName}";
                //response.msg = "success";
                //response.email = user.Email;
                return Json(new { fullName = $"{user.LastName} {user.FirstName}", msg = "success", email = user.Email });

            }
            return Json(new { fullName = "", msg = "failed", email = "" });

        }

        [HttpPost("rentals")]
        [HttpGet("rentals")]
        public async Task<IActionResult> Rentals(int p = 1)
        {
            ViewBag.states = new SelectList(context.States.OrderBy(n => n.StateName), "Id", "StateName");
            ViewBag.propertyTypes = new SelectList(context.PropertyTypes.OrderBy(n => n.PropertyName), "Id", "PropertyName");

            // var rentals = context.Rentals.Where(r => r.StateId == int.Parse(HttpContext.Request.Form["stateId"])).ToListAsync();
            //List<Rental> rentalList = await rentals;
            int pageSize = 3; // number of rental to show per page 
            var rentals = context.Rentals.Include(x => x.image).Skip((p - 1) * pageSize).Take(pageSize); ;

            if (HttpContext.Request.HasFormContentType)
            {
                if (int.Parse(HttpContext.Request.Form["stateId"]) > 0)
                {
                    rentals.Where(x => x.StateId == int.Parse(HttpContext.Request.Form["stateId"]));
                }

                if (int.Parse(HttpContext.Request.Form["lga"]) > 0)
                {
                    rentals.Where(x => x.LgaId == int.Parse(HttpContext.Request.Form["lga"]));
                }

                if (int.Parse(HttpContext.Request.Form["propertyTypeId"]) > 0)
                {
                    rentals.Where(x => x.PropertyTypeId == int.Parse(HttpContext.Request.Form["propertyTypeId"]));
                }

                if (int.Parse(HttpContext.Request.Form["price"]) > 0)
                {
                    rentals.Where(x => x.Rent == decimal.Parse(HttpContext.Request.Form["price"])).OrderByDescending(x => x.Rent);
                }
            }

            // paginate the list 
            ViewBag.PageNumber = p;
            ViewBag.PageRange = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((decimal)context.Rentals.Count() / pageSize);
            //List<Rental> rentalList = await rentals.ToListAsync();
            return View(await rentals.ToListAsync());
        }

        [HttpGet("quick-post")]
        public IActionResult QuickPost() 
        {
            ViewBag.states = new SelectList(context.States.OrderBy(n => n.StateName), "Id", "StateName");
            ViewBag.propertyTypes = new SelectList(context.PropertyTypes.Where(n => n.PropertyName != "Shortlet Rentals"), "Id", "PropertyName");
            //ViewBag.propertyTypes = new SelectList(context.PropertyTypes.OrderBy(n => n.PropertyName), "Id", "PropertyName");
            return View();
        }

        [HttpPost("quick-post")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(RentalViewModel rentalVM)
        {
            // check if the user is logged in 
            if (User.Identity.IsAuthenticated)
            {
                TempData["error"] = "Post a rental add here";
                return RedirectToAction("Rental");
            }

            ViewBag.states = new SelectList(context.States.OrderBy(n => n.StateName), "Id", "StateName");
            ViewBag.propertyTypes = new SelectList(context.PropertyTypes.Where(n => n.PropertyName != "Shortlet Rentals"), "Id", "PropertyName");
            // create new rental
            var rental = new Rental();
            // split the name 
            string id = context.Users.Where(x => x.PhoneNumber == rentalVM.PhoneNumber).Select(x => x.Id).FirstOrDefault();
            if (id == null) // user does not exist 
            {
                string[] name = rentalVM.FullName.Split(" ");
                string address = $"{rentalVM.HouseNumber} {rentalVM.StreetName} {rentalVM.AreaName}, {context.Lgas.Single(s => s.Id == rentalVM.LgaId).LGAName}, {context.States.Single(s => s.Id == rentalVM.StateId).StateName}";
                // add the user 
                // only add the user if the user does not exist 
                AppUser appUser = new AppUser
                {
                    UserName = rentalVM.PhoneNumber,
                    FirstName = name[0],
                    LastName = name[1],
                    Address = address,
                    Email = rentalVM.Email,
                    PhoneNumber = rentalVM.PhoneNumber
                };
                IdentityResult result = await userManager.CreateAsync(appUser, rentalVM.PhoneNumber); // creates the user and set their phone as password 
                if (!result.Succeeded)
                {
                    ViewBag.states = new SelectList(context.States.OrderBy(n => n.StateName), "Id", "StateName");
                    ViewBag.propertyTypes = new SelectList(context.PropertyTypes.Where(n => n.PropertyName != "Shortlet Rentals"), "Id", "PropertyName");

                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View("QuickPost");
                }

                rental.ClientId = appUser.Id; // get user id
                // set the model fields 
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

                // upload the rental image 
                string imgUpload = "no-house.jpg";
                if (rentalVM.ImageUpload != null)
                {
                    string uploadDir = Path.Combine(webHostingEnvironment.WebRootPath, "Media/Rentals");
                    imgUpload = Guid.NewGuid().ToString() + "_" + rentalVM.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadDir, imgUpload);
                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await rentalVM.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                }
                // add the rental to the context, then add the image to the rental context 
                context.Rentals.Add(rental);
                rental.image = new List<Image>() { new Image { RentalId = rental.Id, ImageLocation = imgUpload } }; // add 1 image to the rental
                rental.slug = $"{context.PropertyTypes.Single(p => p.Id == rentalVM.PropertyTypeId).PropertyName}-for-rent-{context.Lgas.Single(s => s.Id == rentalVM.LgaId).LGAName}-{context.States.Single(s => s.Id == rentalVM.StateId).StateName}-{rental.Id}".Replace(" ", "-").ToLower();
                await context.SaveChangesAsync();

                TempData["success"] = "Rental ad posted successfully.";
                return View("QuickPost");

            }

            // user already exist 
            // set the model fields 
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

            //upload the rental image
            string imageName = "no-house.jpg";
            if (rentalVM.ImageUpload != null)
            {
                string uploadDir = Path.Combine(webHostingEnvironment.WebRootPath, "Media/Rentals");
                imageName = Guid.NewGuid().ToString() + "_" + rentalVM.ImageUpload.FileName;
                string filePath = Path.Combine(uploadDir, imageName);
                FileStream fs = new FileStream(filePath, FileMode.Create);
                await rentalVM.ImageUpload.CopyToAsync(fs);
                fs.Close();
            }

            //add the rental to the context, then add the image to the rental context
            rental.ClientId = id;
            context.Rentals.Add(rental);
            rental.image = new List<Image>() { new Image { RentalId = rental.Id, ImageLocation = imageName } }; // add 1 image to the rental
            rental.slug = $"{context.PropertyTypes.Single(p => p.Id == rentalVM.PropertyTypeId).PropertyName}-for-rent-{context.Lgas.Single(s => s.Id == rentalVM.LgaId).LGAName}-{context.States.Single(s => s.Id == rentalVM.StateId).StateName}-{rental.Id}".Replace(" ", "-").ToLower();
            await context.SaveChangesAsync();

            TempData["success"] = "Rental ad posted successfully.";
            return View("QuickPost");

            //if (ModelState.IsValid)
            //{
            //    TempData["success"] = "Rental ad posted successfully.";
            //    return View("QuickPost");
            //}
            //else
            //{
            //    var modelErrors = new List<string>();

            //    foreach (var modelState in ModelState.Values)
            //    {
            //        foreach (var modelError in modelState.Errors)
            //        {
            //            ModelState.AddModelError("", modelError.ErrorMessage);
            //        }
            //    }
            //    TempData["error"] = "Rental ad error.";
            //    return View("QuickPost");
            //}



            //TempData["error"] = "Rental ad error.";
            //return View("QuickPost");
        }

        [HttpGet("rental/{slug}")]
        public IActionResult RentalDetails(string slug)
        {
            var rental = context.Rentals.Include(x => x.image).Include(x => x.Client).Where(x => x.slug == slug).FirstOrDefault();
            return View(rental);
        }


        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
