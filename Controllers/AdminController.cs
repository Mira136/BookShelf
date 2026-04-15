using Microsoft.AspNetCore.Identity;
using BookShelf.Models;
using BookShelf.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookShelf.Data;
using Microsoft.EntityFrameworkCore;

namespace BookShelf.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Dashboard()
        {
            return View();
        }
        public async Task<IActionResult> Users()
        {
            var allUsers = _userManager.Users.ToList();
            var users = new List<ApplicationUser>();

            foreach (var user in allUsers)
            {
                if (!await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    users.Add(user);
                }
            }

            return View(users.OrderByDescending(u => u.CreatedAt).ToList());
        }
        public IActionResult Scoreboard()
        {
            return View();
        }
        public async Task<IActionResult> Books()
        {
            var books = await _db.Books
                .Include(b => b.Category)
                .Include(b => b.Uploader)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return View(books);
        }
        public IActionResult Ebooks()
        {
            var ebooks = _db.Ebooks
                .Include(e => e.Category)
                .Include(e => e.Uploader)
                .OrderByDescending(e => e.CreatedAt)
                .ToList();

            return View(ebooks);
        }
        // ================= NOTIFICATIONS =================

        [Authorize(Roles = "Admin")]
        public IActionResult Notifications()
        {
            var model = new AdminNotificationsViewModel
            {
                Notifications = GetNotificationsFromTempData()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult AddNotification(AdminNotificationsViewModel model)
        {
            var list = GetNotificationsFromTempData();

            if (!string.IsNullOrWhiteSpace(model.NewNotification.Title))
            {
                model.NewNotification.Id = list.Count + 1;
                model.NewNotification.TimeAgo = "Just now";
                list.Insert(0, model.NewNotification);
                SaveNotificationsToTempData(list);
                TempData["NotifSaved"] = "true";
            }

            return RedirectToAction("Notifications");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DeleteNotification(int id)
        {
            var list = GetNotificationsFromTempData();
            list.RemoveAll(n => n.Id == id);
            SaveNotificationsToTempData(list);
            return RedirectToAction("Notifications");
        }

        // ── helpers ───────────────────────────────────────────────────
        private List<NotificationItemViewModel> GetNotificationsFromTempData()
        {
            // Keep TempData alive across redirects
            TempData.Keep("NotifList");
            if (TempData["NotifList"] is string json && !string.IsNullOrEmpty(json))
            {
                try
                {
                    return System.Text.Json.JsonSerializer
                           .Deserialize<List<NotificationItemViewModel>>(json)
                           ?? DefaultNotifications();
                }
                catch { }
            }
            return DefaultNotifications();
        }

        private void SaveNotificationsToTempData(List<NotificationItemViewModel> list)
        {
            TempData["NotifList"] = System.Text.Json.JsonSerializer.Serialize(list);
        }

        private List<NotificationItemViewModel> DefaultNotifications()
        {
            return new List<NotificationItemViewModel>
    {
        new() { Id=1, Title="Order Placed Successfully",   Description="Book order #BK2041 has been confirmed.",                     Type="order",  TimeAgo="2 mins ago"  },
        new() { Id=2, Title="Out for Delivery",            Description="Order #BK2038 is out for delivery.",                         Type="order",  TimeAgo="1 hour ago"  },
        new() { Id=3, Title="Flat 20% Off – Limited Time", Description="Use code READ20 at checkout on 500+ books.",                 Type="offer",  TimeAgo="5 hours ago" },
        new() { Id=4, Title="Price Drop Alert",            Description="\"Deep Work\" in wishlist just dropped in price.",            Type="offer",  TimeAgo="1 day ago"   },
        new() { Id=5, Title="New Books Available",         Description="12 new books matching user interests have been listed.",     Type="system", TimeAgo="2 days ago"  },
        new() { Id=6, Title="Listing Posted Successfully", Description="Book listing \"The Alchemist\" is now live.",                Type="system", TimeAgo="3 days ago"  }
    };
        }
    }
}
