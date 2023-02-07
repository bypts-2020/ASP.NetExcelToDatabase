using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UploadExcel.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<UploadExcelContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("UploadExcelContext") ?? throw new InvalidOperationException("Connection string 'UploadExcelContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
