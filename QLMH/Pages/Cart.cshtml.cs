using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QLMH.Data;
using QLMH.Helpers; 
using QLMH.Models;  
using QLMH.ViewModels; 

namespace QLMH.Pages
{
    public class CartModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public CartModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<CartItem> Cart { get; set; } // Giỏ hàng để hiển thị
        public decimal TotalAmount { get; set; } // Tổng tiền

        // Hàm OnGet: Chỉ để hiển thị giỏ hàng
        public void OnGet()
        {
            Cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart") ?? new List<CartItem>();
            TotalAmount = Cart.Sum(item => item.Total);
        }

        // Hàm OnPostAddToCart: Xử lý khi nhấn nút "Thêm vào giỏ"
        // Chúng ta sẽ dùng "Page Handler"
        public async Task<IActionResult> OnPostAddToCartAsync(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                // Nếu chưa đăng nhập, chuyển họ đến trang Đăng nhập
                // returnUrl sẽ đưa họ quay lại trang sản phẩm này sau khi đăng nhập
                string returnUrl = $"{Request.Path}{Request.QueryString}";
                return RedirectToPage("/Account/Login", new { area = "Identity", returnUrl = returnUrl });
            }
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Lấy giỏ hàng từ Session
            var cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart") ?? new List<CartItem>();

            // Kiểm tra xem hàng đã có trong giỏ chưa
            var cartItem = cart.FirstOrDefault(item => item.ProductId == id);
            if (cartItem != null)
            {
                // Nếu có, tăng số lượng
                cartItem.Quantity++;
            }
            else
            {
                // Nếu chưa, thêm mới
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,

                    // SỬA DÒNG NÀY:
                    // Dùng toán tử 3 ngôi: Nếu SalePrice có giá trị > 0 ?
                    // thì lấy SalePrice, ngược lại : lấy Price
                    Price = (product.SalePrice != null && product.SalePrice > 0)
                    ? product.SalePrice.Value
                    : product.Price,

                    Quantity = 1
                });
            }

            // Lưu giỏ hàng trở lại Session
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);

            // Chuyển hướng về trang giỏ hàng
            return RedirectToPage("Cart");
        }

        
    }
}