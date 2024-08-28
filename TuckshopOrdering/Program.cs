using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TuckshopOrdering.Areas.Identity.Data;
using TuckshopOrdering.Models;
using Stripe;
using Stripe.Checkout;
using System.Collections.Generic;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("TuckshopOrderingSystemConnection") ?? throw new InvalidOperationException("Connection string 'TuckshopOrderingSystemConnection' not found.");

builder.Services.AddDbContext<TuckshopOrderingSystem>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<TuckshopOrderingUser>(options => options.SignIn.RequireConfirmedAccount = false)
	.AddEntityFrameworkStores<TuckshopOrderingSystem>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession(); // added this
builder.Services.AddHttpContextAccessor(); // added this
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<TuckshopOrderingUser>>();

    string fullName = "Admin";
    string email = "admin@jpcatering.com";
    string password = "Passw0rd!";

    if (await userManager.FindByEmailAsync(email) == null)
    {
        var user = new TuckshopOrderingUser();
        user.UserName = email;
        user.Email = email;
        user.FullName = fullName;
        await userManager.CreateAsync(user, password);
    }

}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();


app.MapRazorPages();
app.UseSession(); // added this
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
