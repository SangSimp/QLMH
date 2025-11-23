using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using QLMH.Data;
using QLMH.Models;

namespace QLMH.Pages.Admin.Products
{
    public class DeleteModel : PageModel
    {
        private readonly QLMH.Data.ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment; 

        public DeleteModel(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
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

            var product = await _context.Products.FirstOrDefaultAsync(m => m.Id == id);

            if (product is not null)
            {
                Product = product;

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);

            if (product != null)
            {
                Product = product;

                // ===============================================
                // << LOGIC XÓA FILE ẢNH KHI XÓA SẢN PHẨM >>
                // ===============================================
                if (!string.IsNullOrEmpty(Product.ImageUrl))
                {
                    string filePath = Path.Combine(_webHostEnvironment.WebRootPath, Product.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
                // ===============================================

                _context.Products.Remove(Product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Sản phẩm đã được xóa thành công!";
            }

            return RedirectToPage("./Index");
        }
    }
}
