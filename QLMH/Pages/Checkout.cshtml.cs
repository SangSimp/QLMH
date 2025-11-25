using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QLMH.Data;
using QLMH.Helpers;
using QLMH.Models;
using QLMH.ViewModels; // Chứa CartItem
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLMH.Pages
{
    public class CheckoutModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager; // Dùng để lấy thông tin User

        public CheckoutModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Order Order { get; set; }

        public List<CartItem> Cart { get; set; }
        public decimal Total { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // 1. Lấy giỏ hàng
            Cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");

            if (Cart == null || Cart.Count == 0)
            {
                return RedirectToPage("/Index"); // Giỏ hàng rỗng thì đá về trang chủ
            }

            Total = Cart.Sum(i => i.Quantity * i.Price);

            // 2. TỰ ĐỘNG ĐIỀN THÔNG TIN (AUTO-FILL)
            // Kiểm tra xem user có đang đăng nhập không
            if (User.Identity.IsAuthenticated)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    // Khởi tạo Order với thông tin của User
                    Order = new Order
                    {
                        FullName = currentUser.FullName,   // Lấy Họ tên
                        ShippingAddress = currentUser.Address, // Lấy Địa chỉ
                        PhoneNumber = currentUser.PhoneNumber // Lấy SĐT
                    };
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");
            if (Cart == null || Cart.Count == 0) return RedirectToPage("/Index");

            Total = Cart.Sum(i => i.Quantity * i.Price);

            // Nếu User đã đăng nhập, gắn UserId vào đơn hàng
            if (User.Identity.IsAuthenticated)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    Order.UserId = currentUser.Id;
                }
            }

            // Gán các thông tin tự động
            Order.OrderDate = DateTime.Now;
            Order.TotalAmount = Total;
            Order.OrderStatus = "Pending";

            // Lưu Đơn hàng (Order)
            _context.Orders.Add(Order);
            await _context.SaveChangesAsync();

            // Lưu Chi tiết đơn hàng (OrderDetails)
            foreach (var item in Cart)
            {
                var orderDetail = new OrderDetail
                {
                    OrderId = Order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                };
                _context.OrderDetails.Add(orderDetail);

                // Trừ tồn kho (nếu cần)
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.StockQuantity -= item.Quantity;
                }
            }
            await _context.SaveChangesAsync();

            // Xóa giỏ hàng sau khi đặt thành công
            HttpContext.Session.Remove("cart");

            return RedirectToPage("/OrderConfirmation", new { id = Order.Id });
        }
    }
}