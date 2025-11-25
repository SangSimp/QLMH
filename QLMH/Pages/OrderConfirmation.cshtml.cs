using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace QLMH.Pages
{
    public class OrderConfirmationModel : PageModel
    {
        public int OrderId { get; set; }

        public void OnGet(int id)
        {
            OrderId = id;
        }
    }
}