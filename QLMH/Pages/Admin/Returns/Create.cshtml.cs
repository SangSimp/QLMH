using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using QLMH.Data;
using QLMH.Models;

namespace QLMH.Pages.Admin.Returns
{
    public class CreateModel : PageModel
    {
        private readonly QLMH.Data.ApplicationDbContext _context;

        public CreateModel(QLMH.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public ReturnRequest ReturnRequest { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.ReturnRequests.Add(ReturnRequest);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
