using System.Collections.Generic;
using TKPM.Models;
using TKPM.ViewModels;

namespace TKPM.ViewModels
{
    public class ManageAccountViewModel
    {
        public ChangePasswordViewModel ChangePassword { get; set; }

        public List<OrderViewModel> Orders { get; set; }  // ✅ Thêm danh sách đơn hàng
    }
}
