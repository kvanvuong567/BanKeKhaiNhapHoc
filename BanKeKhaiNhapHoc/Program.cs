using Microsoft.EntityFrameworkCore;
using BanKeKhaiNhapHoc.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BanKeKhaiNhapHocContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BanKeKhaiNhapHocContext")
        ?? throw new InvalidOperationException("Connection string 'BanKeKhaiNhapHocContext' not found.")));

builder.Services.AddControllersWithViews();

builder.Services.AddSession();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
