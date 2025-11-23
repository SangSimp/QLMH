using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using QLMH.Data;
using QLMH.Models;

namespace QLMH.Pages.Admin.Products
{
    public class EditModel : PageModel
    {
        private readonly QLMH.Data.ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EditModel(QLMH.Data.ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [BindProperty]
        public Product Product { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product =  await _context.Products.FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            Product = product;
           ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            if (!ModelState.IsValid)
            {
                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
                return Page();
            }

            // Lấy thông tin sản phẩm HIỆN TẠI từ CSDL để so sánh (quan trọng!)
            var productToUpdate = await _context.Products.FindAsync(Product.Id);

            if (productToUpdate == null)
            {
                return NotFound();
            }

            // Cập nhật các trường từ Product (đã bind từ form) vào productToUpdate
            productToUpdate.Name = Product.Name;
            productToUpdate.Description = Product.Description;
            productToUpdate.Price = Product.Price;
            productToUpdate.SalePrice = Product.SalePrice;
            productToUpdate.CategoryId = Product.CategoryId;
            productToUpdate.StockQuantity = Product.StockQuantity;

            // ===============================================
            // << LOGIC XỬ LÝ TẢI FILE ẢNH LÊN KHI CHỈNH SỬA >>
            // ===============================================
            if (Product.ImageFile != null) // Nếu có ảnh MỚI được tải lên
            {
                // Xóa ảnh cũ nếu có
                if (!string.IsNullOrEmpty(productToUpdate.ImageUrl))
                {
                    string oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, productToUpdate.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // 1. Tạo tên file duy nhất cho ảnh MỚI
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Product.ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // 2. Lưu file vật lý ảnh MỚI vào thư mục wwwroot/images
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Product.ImageFile.CopyToAsync(fileStream);
                }

                // 3. Cập nhật đường dẫn ảnh MỚI vào CSDL
                productToUpdate.ImageUrl = "/images/" + uniqueFileName;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(Product.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            TempData["SuccessMessage"] = "Sản phẩm đã được cập nhật thành công!";
            return RedirectToPage("./Index");
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
