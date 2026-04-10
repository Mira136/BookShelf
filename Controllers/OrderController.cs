using BookShelf.Data;
using BookShelf.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookShelf.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // ================= WISHLIST (teammate's code - kept as is) =================

        public IActionResult Wishlist()
        {
            if (!User.Identity.IsAuthenticated)
                return Redirect("/Account/Login");

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var wishlist = _db.WishlistItems
                .Where(w => w.UserId == userId)
                .Include(w => w.Book)
                .ThenInclude(b => b.Category)
                .ToList();
            return View(wishlist);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleWishlist(int bookId)
        {
            if (!User.Identity.IsAuthenticated)
                return Redirect("/Account/Login");

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var item = _db.WishlistItems
                .FirstOrDefault(w => w.BookId == bookId && w.UserId == userId);

            if (item != null)
                _db.WishlistItems.Remove(item);
            else
                _db.WishlistItems.Add(new WishlistItem
                {
                    BookId = bookId,
                    UserId = userId
                });

            await _db.SaveChangesAsync();
            return Redirect(Request.Headers["Referer"].ToString());
        }

        // ================= ADD TO CART =================

        [Authorize]
        public async Task<IActionResult> AddToCart(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            if (id == 0)
                return Content("Id is 0!");

            var book = await _db.Books.FindAsync(id);
            if (book == null)
                return Content($"Book with ID {id} not found!");

            var existing = await _db.CartItems
                .FirstOrDefaultAsync(c => c.UserId == user.Id
                                       && c.BookId == id);
            if (existing == null)
            {
                _db.CartItems.Add(new CartItem
                {
                    UserId = user.Id,
                    BookId = id,
                    Quantity = 1
                });
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("Cart");
        }

        // ================= CART =================

        [Authorize]
        public async Task<IActionResult> Cart()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var cartItems = await _db.CartItems
                .Include(c => c.Book)
                    .ThenInclude(b => b.Category)
                .Where(c => c.UserId == user.Id)
                .ToListAsync();

            decimal total = cartItems
                .Sum(c => (c.Book?.SellingPrice ?? 0) * c.Quantity);

            ViewBag.Total = total;
            return View(cartItems);
        }

        // ================= REMOVE FROM CART =================

        [Authorize]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var item = await _db.CartItems.FindAsync(id);
            if (item != null)
            {
                _db.CartItems.Remove(item);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Cart");
        }

        // ================= CHECKOUT =================

        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var cartItems = await _db.CartItems
                .Include(c => c.Book)
                .Where(c => c.UserId == user.Id)
                .ToListAsync();

            if (!cartItems.Any())
                return RedirectToAction("Cart");

            decimal total = cartItems
                .Sum(c => (c.Book?.SellingPrice ?? 0) * c.Quantity);

            ViewBag.Total = total;
            ViewBag.User = user;
            return View(cartItems);
        }

        // ================= PLACE ORDER =================

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PlaceOrder()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var cartItems = await _db.CartItems
                .Include(c => c.Book)
                .Where(c => c.UserId == user.Id)
                .ToListAsync();

            if (!cartItems.Any())
                return RedirectToAction("Cart");

            decimal total = cartItems
                .Sum(c => (c.Book?.SellingPrice ?? 0) * c.Quantity);

            // Generate unique order number
            string orderNumber = "BK" + DateTime.Now.Ticks
                .ToString().Substring(10, 5);

            // Create order
            var order = new Order
            {
                UserId = user.Id,
                OrderNumber = orderNumber,
                TotalAmount = total,
                Status = "Pending",
                PaymentMethod = "Cash on Delivery",
                ShippingAddress = user.Address,
                OrderDate = DateTime.Now
            };

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            // Create order items
            foreach (var item in cartItems)
            {
                _db.OrderItems.Add(new OrderItem
                {
                    OrderId = order.Id,
                    BookId = item.BookId,
                    Quantity = item.Quantity,
                    Price = item.Book?.SellingPrice ?? 0
                });
            }

            // Clear cart after order placed
            _db.CartItems.RemoveRange(cartItems);
            await _db.SaveChangesAsync();

            TempData["OrderNumber"] = order.OrderNumber;
            TempData["TotalAmount"] = order.TotalAmount.ToString();

            return RedirectToAction("OrderSuccess");
        }

        // ================= ORDER SUCCESS =================

        public IActionResult OrderSuccess()
        {
            ViewBag.OrderNumber = TempData["OrderNumber"] ?? "BK00000";
            ViewBag.TotalAmount = TempData["TotalAmount"] ?? "0";
            return View();
        }

        // ================= MY ORDERS =================

        [Authorize]
        public async Task<IActionResult> Orders()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var orders = await _db.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Book)
                .Where(o => o.UserId == user.Id)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }
        [Authorize]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            var order = await _db.Orders
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == user.Id);

            if (order == null)
                return NotFound();

            if (order.Status == "Delivered")
                return Content("Delivered orders cannot be cancelled!");

            order.Status = "Cancelled";
            await _db.SaveChangesAsync();

            return RedirectToAction("Orders");
        }
    }
}