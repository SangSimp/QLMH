using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore; // 1. THÊM USING NÀY
using QLMH.Data; // (Thay QLMH b?ng tên Project c?a b?n)

namespace QLMH.Pages.Admin.Dashboard
{
    public class IndexModel : PageModel
    {
        // 2. Tiêm (Inject) DbContext
        private readonly ApplicationDbContext _context;
        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // 3. T?o các bi?n ?? ch?a s? li?u
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalProducts { get; set; }
        public int TotalCustomers { get; set; }

        // 4. Vi?t hàm OnGetAsync ?? l?y d? li?u
        public async Task OnGetAsync()
        {
            // Dùng .CountAsync() ?? ??m t?ng s? ??n hàng
            TotalOrders = await _context.Orders.CountAsync();

            // Dùng .SumAsync() ?? tính t?ng c?t TotalAmount
            TotalRevenue = await _context.Orders.SumAsync(o => o.TotalAmount);

            // ??m t?ng s?n ph?m
            TotalProducts = await _context.Products.CountAsync();

            // ??m t?ng khách hàng (ApplicationUser)
            // (B?n có th? ??i .Users thành .Customers n?u có b?ng riêng)
            TotalCustomers = await _context.Users.CountAsync();
        }
    }
}