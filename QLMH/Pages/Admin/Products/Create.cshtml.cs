using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using QLMH.Data;
using QLMH.Models;

namespace QLMH.Pages.Admin.Products
{
    public class CreateModel : PageModel
    {
        private readonly QLMH.Data.ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CreateModel(QLMH.Data.ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public SelectList CategoryNameSL { get; set; }
        public IActionResult OnGet()
        {
            CategoryNameSL = new SelectList(_context.Categories, "Id", "Name");
            return Page();
        }
        

        [BindProperty]
        public Product Product { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
                return Page();
            }

            if (Product.ImageFile != null)
            {
                // 1. Tạo tên file duy nhất
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images"); // << Thư mục lưu ảnh
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Product.ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // 2. Lưu file vật lý vào thư mục wwwroot/images
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Product.ImageFile.CopyToAsync(fileStream);
                }

                // 3. Lưu đường dẫn tương đối vào CSDL
                Product.ImageUrl = "/images/" + uniqueFileName;
            }

            _context.Products.Add(Product);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Sản phẩm đã được thêm thành công!";
            return RedirectToPage("./Index");
        }
    }
}
