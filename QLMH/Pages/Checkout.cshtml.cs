using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QLMH.Data;
using QLMH.Helpers;
using QLMH.Models;
using QLMH.ViewModels; // Ch?a CartItem
using Microsoft.EntityFrameworkCore; // Cho Transaction

namespace QLMH.Pages
{
    public class CheckoutModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public CheckoutModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Dùng ?? hi?n th? tóm t?t gi? hàng
        public List<CartItem> Cart { get; set; }
        public decimal TotalAmount { get; set; }

        // Dùng ?? nh?n d? li?u t? Form POST
        [BindProperty]
        public Order Order { get; set; }

        public IActionResult OnGet()
        {
            // T?i gi? hàng t? Session
            Cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart") ?? new List<CartItem>();

            if (Cart.Count == 0)
            {
                // N?u gi? hàng tr?ng, không cho checkout, ?á v? trang ch?
                return RedirectToPage("Index");
            }

            TotalAmount = Cart.Sum(item => item.Total);
            return Page();
        }

        // Thêm hàm này vào file QLMH.Pages.CheckoutModel (Checkout.cshtml.cs)

        public async Task<IActionResult> OnPostAsync()
        {
            // T?i l?i gi? hàng và t?ng ti?n
            Cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart") ?? new List<CartItem>();
            if (Cart.Count == 0)
            {
                ModelState.AddModelError("", "Gi? hàng c?a b?n tr?ng.");
            }

            // Ki?m tra xem thông tin nh?p (FullName, Address...) có h?p l? không
            if (!ModelState.IsValid)
            {
                // N?u không h?p l?, t?i l?i t?ng ti?n và hi?n th? l?i trang
                TotalAmount = Cart.Sum(item => item.Total);
                return Page();
            }

            // B?t ??u m?t Transaction (Giao d?ch CSDL)
            // ??m b?o t?t c? cùng thành công, ho?c th?t b?i
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 1. Gán thông tin cho ??n hàng (Order)
                    // (Order ?ã ???c BindProperty t? form)
                    Order.OrderDate = DateTime.Now;
                    Order.TotalAmount = Cart.Sum(item => item.Total);
                    Order.OrderStatus = "Pending"; // Tr?ng thái ch? x? lý

                    // 2. L?u ??n hàng (Order) vào CSDL
                    await _context.Orders.AddAsync(Order);
                    await _context.SaveChangesAsync(); // Ph?i save ? ?ây ?? l?y ???c Order.Id

                    // 3. L?p qua gi? hàng ?? t?o Chi ti?t ??n hàng (OrderDetail)
                    foreach (var cartItem in Cart)
                    {
                        // T?o m?t OrderDetail m?i
                        var orderDetail = new OrderDetail
                        {
                            OrderId = Order.Id, // L?y Id t? Order v?a l?u
                            ProductId = cartItem.ProductId,
                            Quantity = cartItem.Quantity,
                            Price = cartItem.Price // L?u giá t?i th?i ?i?m mua
                        };
                        await _context.OrderDetails.AddAsync(orderDetail);

                        // 4. TR? KHO (Module 10)
                        var product = await _context.Products.FindAsync(cartItem.ProductId);
                        if (product != null)
                        {
                            product.StockQuantity -= cartItem.Quantity;
                            // (B?n có th? thêm ki?m tra n?u StockQuantity < 0 thì báo l?i)
                            _context.Products.Update(product);
                        }
                    }

                    // 5. L?u t?t c? thay ??i (OrderDetails và Products)
                    await _context.SaveChangesAsync();

                    // 6. Cam k?t Transaction (Hoàn t?t)
                    await transaction.CommitAsync();

                    // 7. Xóa gi? hàng kh?i Session
                    HttpContext.Session.Remove("cart");

                    // 8. Chuy?n ??n trang C?m ?n/Thành công
                    // (T?m th?i redirect v? Index, b?n nên t?o trang OrderSuccess)
                    return RedirectToPage("Index", new { message = "??t hàng thành công!" });
                }
                catch (Exception ex)
                {
                    // N?u có l?i ? b?t k? b??c nào (tr? kho, l?u CSDL...)
                    // H?y b? m?i thay ??i
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", $"??t hàng th?t b?i: {ex.Message}");

                    // T?i l?i t?ng ti?n và hi?n th? l?i trang
                    TotalAmount = Cart.Sum(item => item.Total);
                    return Page();
                }
            }
        }
    }
}