namespace QLMH.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; } // Khóa ngoại tới Order
        public int ProductId { get; set; } // Khóa ngoại tới Product

        public int Quantity { get; set; }
        public decimal Price { get; set; } // Lưu lại giá tại thời điểm mua

        // Thuộc tính điều hướng
        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}