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
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public IndexModel(ApplicationDbContext context) { _context = context; }

        public List<Order> Order { get; set; }

        public async Task OnGetAsync()
        {
            Order = await _context.Orders
                                    .OrderByDescending(o => o.OrderDate)
                                    .ToListAsync();
        }
    }
}
