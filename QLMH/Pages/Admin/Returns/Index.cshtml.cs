using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QLMH.Data;
using QLMH.Models;

namespace QLMH.Pages.Admin.Returns
{
    public class IndexModel : PageModel
    {
        private readonly QLMH.Data.ApplicationDbContext _context;

        public IndexModel(QLMH.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<ReturnRequest> ReturnRequest { get;set; } = default!;

        public async Task OnGetAsync()
        {
            ReturnRequest = await _context.ReturnRequests.ToListAsync();
            ReturnRequest = await _context.ReturnRequests
        .Include(r => r.Order)        // Tải thông tin Order
        .Include(r => r.Order!.User)  // Tải thông tin User từ Order
        .ToListAsync();
        }
    }
}
