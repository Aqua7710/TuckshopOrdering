using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TuckshopOrdering.Areas.Identity.Data;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("TuckshopOrderingSystemConnection") ?? throw new InvalidOperationException("Connection string 'TuckshopOrderingSystemConnection' not found.");

builder.Services.AddDbContext<TuckshopOrderingSystem>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<TuckshopOrderingUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<TuckshopOrderingSystem>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.MapRazorPages();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Menus}/{action=Index}/{id?}");

app.Run();
