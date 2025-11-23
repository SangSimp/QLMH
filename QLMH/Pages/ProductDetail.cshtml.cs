using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QLMH.Data;
using QLMH.Helpers; // << THÊM VÀO (Giả sử bạn có SessionHelper)
using QLMH.Models;
using System.Collections.Generic; // << THÊM VÀO
using System.Linq; // << THÊM VÀO
using QLMH.ViewModels; // << THÊM DÒNG NÀY

namespace QLMH.Pages
{
    public class ProductDetailModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductDetailModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Product Product { get; set; }
        public List<Review> Reviews { get; set; }

        [BindProperty]
        public Review NewReview { get; set; }

        // === HÀM LẤY DỮ LIỆU GỐC (GIỮ NGUYÊN) ===
        public async Task<IActionResult> OnGetAsync(int id)
        {
            Product = await _context.Products
                                .Include(p => p.Category)
                                .FirstOrDefaultAsync(p => p.Id == id);
            if (Product == null)
            {
               
                TempData["ErrorMessage"] = "Không tìm thấy sản phẩm này.";
                return RedirectToPage("./Index"); 
            }

            Reviews = await _context.Reviews
                                .Where(r => r.ProductId == id)
                                .Include(r => r.User)
                                .OrderByDescending(r => r.ReviewDate)
                                .ToListAsync();

            return Page();
        }

        // === HÀM THÊM GIỎ HÀNG (MỚI) ===
        public async Task<IActionResult> OnPostAsync(int id, int quantity)
        {
            // Bước 1: Luôn tải lại "Product" để tránh lỗi "NullReferenceException"
            // nếu chúng ta cần "return Page()"
            Product = await _context.Products
                                .Include(p => p.Category)
                                .FirstOrDefaultAsync(p => p.Id == id);
            if (Product == null)
            {
                return NotFound();
            }

            // Bước 2: Kiểm tra đăng nhập (đã làm)
            if (!User.Identity.IsAuthenticated)
            {
                string returnUrl = $"{Request.Path}{Request.QueryString}";
                return RedirectToPage("/Account/Login", new { area = "Identity", returnUrl = returnUrl });
            }

            // Bước 3: Kiểm tra số lượng hợp lệ
            if (quantity <= 0)
            {
                // Nếu số lượng không hợp lệ, tải lại Reviews và trả về trang
                Reviews = await _context.Reviews
                                        .Where(r => r.ProductId == id)
                                        .Include(r => r.User)
                                        .OrderByDescending(r => r.ReviewDate)
                                        .ToListAsync();
                ModelState.AddModelError("", "Số lượng phải lớn hơn 0");
                return Page(); // Trả về trang (Lần này Product KHÔNG bị null)
            }

            // Bước 4: Xử lý Logic Giỏ hàng
            var cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart") ?? new List<CartItem>();

            var cartItem = cart.FirstOrDefault(c => c.ProductId == id);
            if (cartItem == null)
            {
                // Thêm mới
                cart.Add(new CartItem
                {
                    ProductId = Product.Id,
                    ProductName = Product.Name,
                    Price = (Product.SalePrice ?? Product.Price), // Lấy giá sale nếu có
                    Quantity = quantity,
                    ImageUrl = Product.ImageUrl
                });
            }
            else
            {
                // Cập nhật số lượng
                cartItem.Quantity += quantity;
            }

            // Lưu cart vào Session
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);

            // Chuyển đến trang Giỏ hàng
            return RedirectToPage("/Cart");
        }

        // === HÀM THÊM REVIEW (GIỮ NGUYÊN) ===
        public async Task<IActionResult> OnPostAddReviewAsync(int id)
        {
            // (Code OnPostAddReviewAsync của bạn)
            if (!User.Identity.IsAuthenticated)
            {
                string returnUrl = $"{Request.Path}{Request.QueryString}";
                return RedirectToPage("/Account/Login", new { area = "Identity", returnUrl = returnUrl });
            }

            ModelState.Remove("NewReview.ProductId");
            ModelState.Remove("NewReview.UserId");

            if (!ModelState.IsValid)
            {
                Product = await _context.Products
                                        .Include(p => p.Category)
                                        .FirstOrDefaultAsync(p => p.Id == id);
                Reviews = await _context.Reviews
                                        .Where(r => r.ProductId == id)
                                        .Include(r => r.User)
                                        .OrderByDescending(r => r.ReviewDate)
                                        .ToListAsync();
                return Page();
            }

            var userId = _userManager.GetUserId(User);

            NewReview.ProductId = id;
            NewReview.UserId = userId;
            NewReview.ReviewDate = DateTime.Now;

            _context.Reviews.Add(NewReview);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { id = id });
        }
    }
}