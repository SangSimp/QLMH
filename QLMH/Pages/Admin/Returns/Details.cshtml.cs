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
    public class DetailsModel : PageModel
    {
        private readonly QLMH.Data.ApplicationDbContext _context;

        public DetailsModel(QLMH.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public ReturnRequest ReturnRequest { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var returnrequest = await _context.ReturnRequests.FirstOrDefaultAsync(m => m.Id == id);

            if (returnrequest is not null)
            {
                ReturnRequest = returnrequest;

                return Page();
            }

            return NotFound();
        }
    }
}
