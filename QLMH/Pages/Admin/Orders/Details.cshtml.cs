using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QLMH.Data;
using QLMH.Models;

namespace QLMH.Pages.Admin.Orders
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public DetailsModel(ApplicationDbContext context) { _context = context; }

        public Order Order { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Order = await _context.Orders
                .Include(o => o.OrderDetails)         // Nối bảng OrderDetails
                .ThenInclude(od => od.Product)        // Từ OrderDetails nối bảng Product
                .FirstOrDefaultAsync(o => o.Id == id); // Tìm theo Id

            if (Order == null) return NotFound();
            return Page();
        }
    }
}
