using BookShelf.Models;
using BookShelf.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookShelf.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _env;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IWebHostEnvironment env)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _env = env;
        }

        // ================= LOGIN =================

        public async Task<IActionResult> Login()
        {
            // If already logged in
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);

                if (user != null)
                {
                    if (await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // ===== ADMIN LOGIN (hardcoded as before) =====
            if (model.Email == "admin@gmail.com" && model.Password == "1234")
            {
                // Find or create admin user in DB
                var adminUser = await _userManager.FindByEmailAsync("admin@gmail.com");
                if (adminUser == null)
                {
                    adminUser = new ApplicationUser
                    {
                        UserName = "admin@gmail.com",
                        Email = "admin@gmail.com",
                        FirstName = "Admin",
                        LastName = "User",
                        Gender = "N/A",
                        Address = "N/A",
                        EmailConfirmed = true
                    };
                    await _userManager.CreateAsync(adminUser, "Admin@1234");
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                }

                await _signInManager.SignInAsync(adminUser, model.RememberMe);
                return RedirectToAction("Dashboard", "Admin");
            }

            // ===== NORMAL USER LOGIN =====
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                await _signInManager.SignInAsync(user, model.RememberMe);

                if (await _userManager.IsInRoleAsync(user, "Admin"))
                    return RedirectToAction("Dashboard", "Admin");

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid email or password");
            return View(model);
        }

        // ================= LOGOUT =================

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // ================= REGISTER =================

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Check if email already exists
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Email is already registered.");
                return View(model);
            }

            // Create new user
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.MobileNo,
                Gender = model.Gender,
                Address = model.Address,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Assign User role
                await _userManager.AddToRoleAsync(user, "User");

                // Auto login after register
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            // Show errors from Identity
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

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
                ViewBag.Message = "Reset link sent to your email!";
            return View(model);
        }

        // ================= USER PROFILE =================

        [Authorize(Roles = "User")]
        public async Task<IActionResult> UserProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            var model = new UserProfileViewModel
            {
                Name = user.FullName,
                Username = user.Email ?? string.Empty,
                Number = user.PhoneNumber ?? string.Empty,
                ProfileImage = user.ProfileImagePath ?? "reg_profile.png"
            };

            return View(model);
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

        // ================= EDIT USER PROFILE =================

        [Authorize(Roles = "User")]
        public async Task<IActionResult> EditUserProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            var model = new EditUserProfileViewModel
            {
                Name = user.FullName,
                Number = user.PhoneNumber ?? string.Empty,
                ExistingPhotoPath = user.ProfileImagePath ?? "reg_profile.png"
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> EditUserProfile(EditUserProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            // Update photo if new one uploaded
            if (model.Photo != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "images");
                string fileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Photo.CopyToAsync(fileStream);
                }
                user.ProfileImagePath = fileName;
            }

            // Update name and phone
            var nameParts = model.Name?.Split(' ', 2) ?? new[] { "", "" };
            user.FirstName = nameParts[0];
            user.LastName = nameParts.Length > 1 ? nameParts[1] : "";
            user.PhoneNumber = model.Number;

            await _userManager.UpdateAsync(user);

            return RedirectToAction("UserProfile");
        }

        // ================= NOTIFICATIONS =================

        public IActionResult Notifications() => View();

        // ================= ABOUT US =================

        public IActionResult AdminAboutUs()
        {
            var model = new AboutUsViewModel
            {
                OurStory = "Our bookstore was created for people who want to buy and sell books at affordable prices.",
                OurMission = "To provide quality books at affordable prices and deliver them quickly to readers.",
                OurVision = "To become a trusted online bookstore for readers everywhere.",
                WhyChooseUs = "Trusted Online Bookstore | Affordable Prices | Simple Shopping Experience | Great Customer Support"
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AdminAboutUs(AboutUsViewModel model)
        {
            if (ModelState.IsValid)
                TempData["AboutSaved"] = "true";
            return View(model);
        }

        public IActionResult About()
        {
            var model = new AboutViewModel
            {
                Title = "About Our BookShelf",
                Subtitle = "Your Trusted Place to Discover and buy books online.",
                StoryDescription = "Our bookstore was created for people who want to buy and sell books at affordable prices.",
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
                    new TeamMember { Name = "Tanvi",  Email = "tanvi123@gmail.com"  },
                    new TeamMember { Name = "Ayushi", Email = "ayushi123@gmail.com" },
                    new TeamMember { Name = "Mirali", Email = "mira123@gmail.com"   }
                },
                OfferItems = new List<OfferItem>
                {
                    new OfferItem { Title = "Wide Selection",       ImageUrl = "/images/books1.jpg" },
                    new OfferItem { Title = "Easy Online Ordering", ImageUrl = "/images/books2.jpg" },
                    new OfferItem { Title = "Nominal Price",        ImageUrl = "/images/books3.jpg" }
                }
            };
            return View(model);
        }
    }
}