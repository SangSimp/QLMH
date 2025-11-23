using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using QLMH.Data; // Namespace chứa ApplicationUser
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace QLMH.Areas.Admin.Pages.Users
{
    public class CreateModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CreateModel(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public SelectList RolesList { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Email là bắt buộc")]
            [EmailAddress]
            public string Email { get; set; }

            [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu ít nhất 6 ký tự")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Required(ErrorMessage = "Họ tên là bắt buộc")]
            public string FullName { get; set; }

            [Required(ErrorMessage = "Vui lòng chọn quyền")]
            public string Role { get; set; }
        }

        public void OnGet()
        {
            // Lấy danh sách Role để đổ vào dropdown
            RolesList = new SelectList(_roleManager.Roles, "Name", "Name");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    FullName = Input.FullName,
                    EmailConfirmed = true // Admin tạo thì mặc định xác thực luôn
                };

                // 1. Tạo User mới
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    // 2. Gán quyền (Role)
                    if (!string.IsNullOrEmpty(Input.Role))
                    {
                        await _userManager.AddToRoleAsync(user, Input.Role);
                    }

                    TempData["SuccessMessage"] = $"Tạo tài khoản {user.Email} thành công!";
                    return RedirectToPage("./Index");
                }

                // Nếu lỗi (ví dụ email trùng)
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Nếu thất bại, load lại dropdown role
            RolesList = new SelectList(_roleManager.Roles, "Name", "Name");
            return Page();
        }
    }
}