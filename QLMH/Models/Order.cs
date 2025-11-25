using QLMH.Data;
using System.ComponentModel.DataAnnotations;

namespace QLMH.Models
{
    public class Order
    {
        public Order()
        {
            OrderDetails = new List<OrderDetail>();
        }
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Tên")]
        [Display(Name = "Họ và Tên")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Địa chỉ")]
        [Display(Name = "Địa chỉ giao hàng")]
        public string ShippingAddress { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập SĐT")]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }

        // Thông tin đơn hàng
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string? OrderStatus { get; set; } 

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }

       
    }
}