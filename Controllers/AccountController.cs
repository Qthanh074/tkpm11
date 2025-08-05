using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TKPM.Data;
using TKPM.Models;
using TKPM.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using TKPM.ViewModels;

namespace TKPM.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context; // ✅ THÊM DÒNG NÀY

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context) // ✅ TIÊM CONTEXT
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context; // ✅ LƯU CONTEXT
        }

        // ✅ Trang quản lý tài khoản
        public async Task<IActionResult> Manage()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");

            // ✅ Lấy danh sách đơn hàng của user
            var orders = await _context.Orders
                .Where(o => o.UserId == user.Id)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderViewModel
                {
                    Id = o.Id,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status
                }).ToListAsync();

            var viewModel = new ManageAccountViewModel
            {
                ChangePassword = new ChangePasswordViewModel { FullName = user.FullName },
                Orders = orders
            };

            return View(viewModel);
        }

        // ✅ Đổi mật khẩu
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Manage");

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                TempData["Success"] = "✅ Đổi mật khẩu thành công!";
                return RedirectToAction("Manage");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return RedirectToAction("Manage");
        }

        // ✅ Đăng nhập, Đăng ký, Đăng xuất giữ nguyên như bạn đã viết
    }
}
