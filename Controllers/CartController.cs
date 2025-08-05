using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TKPM.Data;
using TKPM.Models;

namespace TKPM.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ✅ Hiển thị giỏ hàng của user
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var cartItems = await _context.CartItems
                                          .Include(c => c.Product)
                                          .Where(c => c.UserId == user.Id)
                                          .ToListAsync();
            return View(cartItems);
        }

        // ✅ Thêm sản phẩm vào giỏ
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var user = await _userManager.GetUserAsync(User);
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return NotFound();

            var existingItem = await _context.CartItems
                                             .FirstOrDefaultAsync(c => c.ProductId == productId && c.UserId == user.Id);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                existingItem.Price = existingItem.Quantity * product.Price;
            }
            else
            {
                _context.CartItems.Add(new CartItem
                {
                    ProductId = product.Id,
                    Quantity = quantity,
                    Price = product.Price * quantity,
                    UserId = user.Id
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // ✅ Xóa sản phẩm khỏi giỏ
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var item = await _context.CartItems.FindAsync(id);
            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        // ✅ Trang thanh toán
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var user = await _userManager.GetUserAsync(User);
            var cartItems = await _context.CartItems.Include(c => c.Product)
                                                    .Where(c => c.UserId == user.Id)
                                                    .ToListAsync();
            return View(cartItems);
        }

        // ✅ Xác nhận thanh toán → tạo Order và OrderItem
        [HttpPost]
        public async Task<IActionResult> CheckoutConfirm(string fullName, string address, string phone)
        {
            var user = await _userManager.GetUserAsync(User);
            var cartItems = await _context.CartItems.Include(c => c.Product)
                                                    .Where(c => c.UserId == user.Id)
                                                    .ToListAsync();

            if (!cartItems.Any()) return RedirectToAction("Index");

            // Tạo Order
            var order = new Order
            {
                UserId = user.Id,
                OrderDate = DateTime.Now,
                TotalAmount = cartItems.Sum(c => c.Price),
                Status = "Pending"
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Thêm OrderItem
            foreach (var item in cartItems)
            {
                _context.OrderItems.Add(new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                });
            }

            // Xóa giỏ hàng sau khi đặt hàng
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            TempData["Success"] = "✅ Đặt hàng thành công!";
            return RedirectToAction("Manage", "Account");
        }
    }
}
