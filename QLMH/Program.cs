using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QLMH.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>() // Thêm .AddRoles<IdentityRole>() để có thể dùng Role
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddAuthorization(options =>
{
    // 1. Tạo chính sách tên "AdminOnly"
    // Chính sách này yêu cầu người dùng phải có Role "Admin"
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));

    // 2. Tạo chính sách tên "AdminOrStaff"
    // Chính sách này yêu cầu người dùng có Role "Admin" HOẶC "Staff"
    options.AddPolicy("AdminOrStaff", policy =>
        policy.RequireRole("Admin", "Staff"));
});
builder.Services.AddRazorPages(options =>
{
    // === YÊU CẦU CHUNG ===
    // (Bất kỳ ai truy cập thư mục /Admin phải đăng nhập)
    options.Conventions.AuthorizeFolder("/Admin");

    // === PHÂN QUYỀN CHI TIẾT (Dùng tên Policy đã tạo) ===

    // 1. NHÂN VIÊN (Staff) & Admin (Dùng Policy "AdminOrStaff")
    options.Conventions.AuthorizeFolder("/Admin/Categories", "AdminOrStaff");
    options.Conventions.AuthorizeFolder("/Admin/Products", "AdminOrStaff");
    options.Conventions.AuthorizeFolder("/Admin/Orders", "AdminOrStaff");

    // 2. CHỈ ADMIN (Chủ shop) (Dùng Policy "AdminOnly")
    options.Conventions.AuthorizeFolder("/Admin/Dashboard", "AdminOnly");
    options.Conventions.AuthorizeFolder("/Admin/Returns", "AdminOnly");

    // 3. KHÁCH HÀNG (Yêu cầu đăng nhập)
    options.Conventions.AuthorizePage("/Cart");
    options.Conventions.AuthorizePage("/Checkout");
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireRole("Admin"));
});

builder.Services.AddDistributedMemoryCache(); 
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    // 1. Tạo Role "Admin" nếu nó chưa tồn tại
    string adminRole = "Admin";
    if (!await roleManager.RoleExistsAsync(adminRole))
    {
        await roleManager.CreateAsync(new IdentityRole(adminRole));
    }

    // 2. Tạo User "admin@admin.com" nếu chưa tồn tại
    var adminUser = new ApplicationUser
    {
        UserName = "admin@admin.com",
        Email = "admin@admin.com",
        FullName = "Quản Trị Viên",
        EmailConfirmed = true // Xác thực email luôn
    };

    var user = await userManager.FindByEmailAsync(adminUser.Email);
    if (user == null)
    {
        // Mật khẩu là "Admin@123"
        await userManager.CreateAsync(adminUser, "Admin@123");
        // Gán Role "Admin" cho user này
        await userManager.AddToRoleAsync(adminUser, adminRole);
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseSession();  

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
