using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QLMH.Data; // (Kiểm tra namespace của bạn)
using QLMH.Models; // (Kiểm tra namespace của bạn)
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System; // Thêm thư viện Math

namespace QLMH.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Product> Products { get; set; }
        public string PageTitle { get; set; }
        public string Message { get; set; }

        // === BIẾN MỚI CHO PHÂN TRANG ===
        public int TotalPages { get; set; } // Tổng số trang

        [BindProperty(SupportsGet = true)]
        public int PageNum { get; set; } = 1; // Số trang hiện tại (mặc định là 1)

        // === CÁC BIẾN LỌC (GIỮ NGUYÊN) ===
        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? CategoryId { get; set; }


        public async Task OnGetAsync()
        {
            Message = TempData["SuccessMessage"] as string;

            // Bắt đầu truy vấn (chưa chạy)
            var query = _context.Products
                                .Include(p => p.Category)
                                .Include(p => p.Reviews)
                                .AsQueryable();

            bool isFiltering = false;

            // Lọc 1: Nếu có CategoryId
            if (CategoryId.HasValue && CategoryId > 0)
            {
                query = query.Where(p => p.CategoryId == CategoryId.Value);
                var category = await _context.Categories.FindAsync(CategoryId.Value);
                PageTitle = $"Danh mục: {category?.Name}";
                isFiltering = true;
            }

            // Lọc 2: Nếu có SearchString
            if (!string.IsNullOrEmpty(SearchString))
            {
                query = query.Where(p => p.Name.Contains(SearchString));
                PageTitle = $"Kết quả tìm: '{SearchString}'";
                isFiltering = true;
            }

            // Xử lý trang chủ (Không lọc)
            if (!isFiltering)
            {
                PageTitle = "Tất cả Sản phẩm";
            }

            // === LOGIC PHÂN TRANG MỚI ===
            int pageSize = 8; // Số sản phẩm mỗi trang (bạn có thể đổi thành 12)

            // 1. Đếm tổng số sản phẩm (SAU KHI LỌC)
            var totalCount = await query.CountAsync();

            // 2. Tính tổng số trang
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            // 3. Đảm bảo PageNum hợp lệ
            PageNum = Math.Max(1, Math.Min(PageNum, TotalPages));

            // 4. Lấy sản phẩm cho trang hiện tại
            query = query.OrderByDescending(p => p.Id) // Luôn sắp xếp trước khi Skip
                         .Skip((PageNum - 1) * pageSize) // Bỏ qua các trang trước
                         .Take(pageSize); // Chỉ lấy 8 sản phẩm

            // 5. Chạy truy vấn cuối cùng
            Products = await query.ToListAsync();
        }
    }
}