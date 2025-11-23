using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QLMH.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLMH.Areas.Admin.Pages.Users
{
    // Class phụ để chứa dữ liệu hiển thị
    public class UserViewModel
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string RoleNames { get; set; } // Chuỗi chứa tên các quyền (VD: "Admin, Staff")
    }

    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public List<UserViewModel> UserList { get; set; }

        public async Task OnGetAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            UserList = new List<UserViewModel>();

            foreach (var user in users)
            {
                // Lấy danh sách quyền của user này
                var roles = await _userManager.GetRolesAsync(user);

                UserList.Add(new UserViewModel
                {
                    Id = user.Id,
                    Username = user.UserName,
                    FullName = user.FullName, // Hiển thị tên thật
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    RoleNames = string.Join(", ", roles) // Nối các quyền lại thành chuỗi
                });
            }
        }
    }
}