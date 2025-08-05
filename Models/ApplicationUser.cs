using Microsoft.AspNetCore.Identity;

namespace TKPM.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Bạn có thể thêm thuộc tính bổ sung nếu cần
        public string? FullName { get; set; }
    }
}
