using QLMH.Data;
using System.ComponentModel.DataAnnotations;

namespace QLMH.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int ProductId { get; set; } 

        public string UserId { get; set; } 

        [Required(ErrorMessage = "Vui lòng chọn số sao")]
        [Range(1, 5, ErrorMessage = "Vui lòng chọn từ 1 đến 5 sao")]
        [Display(Name = "Đánh giá")]
        public int Rating { get; set; } 

        [Display(Name = "Bình luận")]
        public string? Comment { get; set; }

        public DateTime ReviewDate { get; set; } 

        public Product? Product { get; set; }
        public ApplicationUser? User { get; set; }
    }
}