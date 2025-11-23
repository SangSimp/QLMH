using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QLMH.Data;
using QLMH.Models;

namespace QLMH.Pages
{
    public class ReturnRequestModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public ReturnRequestModel(ApplicationDbContext context) { _context = context; }

        [BindProperty]
        public ReturnRequest Request { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // (Nâng cao): Ki?m tra xem OrderId có t?n t?i không
            // var orderExists = await _context.Orders.AnyAsync(o => o.Id == Request.OrderId);
            // if (!orderExists) { ... báo l?i ... }

            Request.RequestDate = DateTime.Now;
            Request.Status = "Pending"; // Ch? x? lý

            _context.ReturnRequests.Add(Request);
            await _context.SaveChangesAsync();

            // Chuy?n v? trang ch? (ho?c 1 trang c?m ?n)
            return RedirectToPage("Index", new { message = "G?i yêu c?u ??i tr? thành công!" });
        }
    }
}