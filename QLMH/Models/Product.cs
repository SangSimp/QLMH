using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLMH.Models
{
    public class Product
    {
        public Product()
        {
            Reviews = new List<Review>();
        }
        public int Id { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập Tên sản phẩm.")]
        [Display(Name = "Tên sản phẩm")]
        public string Name { get; set; }
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập Giá tiền.")]
        [Display(Name = "Giá tiền")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Số lượng tồn kho là bắt buộc")]
        [Display(Name = "Số lượng tồn kho")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng không hợp lệ")]
        public int StockQuantity { get; set; }

      
        [Required(ErrorMessage = "Vui lòng chọn Loại sản phẩm.")]
        [Display(Name = "Loại sản phẩm")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        [Display(Name = "Giá khuyến mãi")]
        public decimal? SalePrice { get; set; }
        [Display(Name = "URL Hình ảnh")]
        public string? ImageUrl { get; set; }
        public ICollection<Review> Reviews { get; set; }
        [NotMapped] 
        [Display(Name = "Chọn hình ảnh")]
        public IFormFile? ImageFile { get; set; }
       
    }
}
