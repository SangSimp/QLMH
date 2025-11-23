using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using QLMH.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLMH.Areas.Admin.Pages.Users
{
    public class EditModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public EditModel(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public ApplicationUser CurrentUser { get; set; }

        [BindProperty]
        public string SelectedRole { get; set; } // Role được chọn từ Dropdown

        public SelectList RolesList { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null) return NotFound();

            CurrentUser = await _userManager.FindByIdAsync(id);
            if (CurrentUser == null) return NotFound();

            // Lấy role hiện tại
            var userRoles = await _userManager.GetRolesAsync(CurrentUser);
            SelectedRole = userRoles.FirstOrDefault(); // Lấy role đầu tiên (nếu có)

            // Lấy danh sách tất cả Role để đổ vào Dropdown
            RolesList = new SelectList(_roleManager.Roles.ToList(), "Name", "Name");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.FindByIdAsync(CurrentUser.Id);
            if (user == null) return NotFound();

            // 1. Lấy các role cũ
            var oldRoles = await _userManager.GetRolesAsync(user);

            // 2. Xóa role cũ
            if (oldRoles.Count > 0)
            {
                await _userManager.RemoveFromRolesAsync(user, oldRoles);
            }

            // 3. Thêm role mới (nếu đã chọn)
            if (!string.IsNullOrEmpty(SelectedRole))
            {
                await _userManager.AddToRoleAsync(user, SelectedRole);
            }

            TempData["SuccessMessage"] = "Cập nhật quyền thành công!";
            return RedirectToPage("./Index");
        }
    }
}