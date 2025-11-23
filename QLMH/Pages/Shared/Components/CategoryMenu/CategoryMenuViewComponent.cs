using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLMH.Data; // (Thay QLMH bằng tên project của bạn)

namespace QLMH.Pages.Shared.Components.CategoryMenu
{
    public class CategoryMenuViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public CategoryMenuViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hàm này sẽ được gọi
        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Lấy 3 danh mục từ CSDL
            var categories = await _context.Categories.Take(3).ToListAsync();
            return View(categories); // Trả về View với 3 danh mục
        }
    }
}