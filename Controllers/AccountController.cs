using BookShelf.Models;
using BookShelf.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BookShelf.Controllers
{
    public class AccountController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public AccountController(IWebHostEnvironment env)
        {
            _env = env;
        }

        // ================= LOGIN =================

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // ===== ADMIN LOGIN =====
            if (model.Email == "admin@gmail.com" && model.Password == "1234")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "Admin"),
                    new Claim(ClaimTypes.Role, "Admin")
                };

                var identity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal);

                return RedirectToAction("Dashboard", "Admin");
            }

            // ===== NORMAL USER LOGIN (Temporary logic) =====
            if (!string.IsNullOrEmpty(model.Email) &&
                !string.IsNullOrEmpty(model.Password))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, model.Email),
                    new Claim(ClaimTypes.Role, "User")
                };

                var identity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid email or password");
            return View(model);
        }

        // ================= LOGOUT =================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Redirect to guest dashboard
            return RedirectToAction("Index", "Home");
        }

        // ================= REGISTER =================

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // TODO: Save to database later
                return RedirectToAction("Login");
            }

            return View(model);
        }

        // ================= FORGOT PASSWORD =================

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Message = "Reset link sent to your email!";
            }

            return View(model);
        }

        // ================= USER PROFILE =================

        [Authorize(Roles = "User")]
        public IActionResult UserProfile()
        {
            var user = new UserProfileViewModel
            {
                Name = TempData["Name"]?.ToString() ?? "Riya Patel",
                Username = User.Identity?.Name,
                Number = TempData["Number"]?.ToString() ?? "9946457894",
                Credit = TempData["Credit"] != null ? Convert.ToInt32(TempData["Credit"]) : 2,
                ProfileImage = TempData["Photo"]?.ToString() ?? "reg_profile.png"
            };

            TempData.Keep();

            return View(user);
        }

        // ================= ADMIN PROFILE =================

        [Authorize(Roles = "Admin")]
        public IActionResult AdminProfile()
        {
            var admin = new AdminProfileViewModel
            {
                Name = "Admin",
                Username = "admin@gmail.com",
                Number = "9946457894"
            };

            return View(admin);
        }

        // ================= NOTIFICATIONS =================

        public IActionResult Notifications()
        {
            return View();
        }
        public IActionResult AdminAboutUs()
        {
            return View();
        }

        // ================= ABOUT US =================

        public IActionResult About()
        {
            var model = new AboutViewModel
            {
                Title = "About Our BookShelf",
                Subtitle = "Your Trusted Place to Discover and buy books online.",

                StoryDescription = "Our bookstore was created for people who want to buy and sell books at affordable prices. We built this platform to help users easily find new and second-hand books. Anyone can sell their books at a nominal price, making books more accessible and budget-friendly for everyone.",

                Mission = "To provide quality books at affordable prices and deliver them quickly to readers.",

                Vision = "To become a trusted online bookstore for readers everywhere.",

                WhyChooseUsPoints = new List<string>
                {
                    "Trusted Online Bookstore",
                    "Affordable Prices",
                    "Simple Shopping Experience",
                    "Great Customer Support"
                },

                TeamMembers = new List<TeamMember>
                {
                    new TeamMember { Name = "Tanvi", Email = "tanvi123@gmail.com" },
                    new TeamMember { Name = "Ayushi", Email = "ayushi123@gmail.com" },
                    new TeamMember { Name = "Mirali", Email = "mira123@gmail.com" }
                },

                OfferItems = new List<OfferItem>
                {
                    new OfferItem { Title = "Wide Selection", ImageUrl = "/images/books1.jpg" },
                    new OfferItem { Title = "Easy Online Ordering", ImageUrl = "/images/books2.jpg" },
                    new OfferItem { Title = "Nominal Price", ImageUrl = "/images/books3.jpg" }
                }
            };

            return View(model);
        }

        // ================= EDIT USER PROFILE =================

        [Authorize(Roles = "User")]
        public IActionResult EditUserProfile()
        {
            var model = new EditUserProfileViewModel
            {
                Id = 1,
                Name = "Riya Patel",
                Number = "9946457894",
                Credit = 2,
                ExistingPhotoPath = TempData["Photo"]?.ToString() ?? "reg_profile.png"
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public IActionResult EditUserProfile(EditUserProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                string photoName = model.ExistingPhotoPath;

                if (model.Photo != null)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "images");
                    string fileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                    string filePath = Path.Combine(uploadsFolder, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        model.Photo.CopyTo(fileStream);
                    }

                    photoName = fileName;
                }

                TempData["Name"] = model.Name;
                TempData["Number"] = model.Number;
                TempData["Credit"] = model.Credit;
                TempData["Photo"] = photoName;

                // Redirect to dashboard instead of profile
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }
    }
}