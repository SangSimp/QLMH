using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QLMH.Data;
using QLMH.Models;

namespace QLMH.Pages.Admin.Categories
{
    public class IndexModel : PageModel
    {
        private readonly QLMH.Data.ApplicationDbContext _context;

        public IndexModel(QLMH.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Category> Category { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Category = await _context.Categories.ToListAsync();
        }
    }
}
