using System.ComponentModel.DataAnnotations;

namespace TKPM.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string Description { get; set; } = string.Empty;

        // ✅ Thuộc tính để lưu link ảnh sản phẩm
        public string ImageUrl { get; set; } = "/images/default.png";
    }
}
