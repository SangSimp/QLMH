using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLMH.Models
{
    public class ReturnRequest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Mã đơn hàng")]
        [Display(Name = "Mã Đơn hàng (OrderId)")]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập lý do")]
        [Display(Name = "Lý do đổi trả")]
        public string Reason { get; set; }

        [Display(Name = "Trạng thái")]
        public string Status { get; set; } 

        public DateTime RequestDate { get; set; }
        [ForeignKey("OrderId")]
        public Order? Order { get; set; }

    }
}