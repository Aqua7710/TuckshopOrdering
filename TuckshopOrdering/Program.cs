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
using NuGet.DependencyResolver;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("TuckshopOrderingSystemConnection") ?? throw new InvalidOperationException("Connection string 'TuckshopOrderingSystemConnection' not found.");

builder.Services.AddDbContext<TuckshopOrderingSystem>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<TuckshopOrderingUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<TuckshopOrderingSystem>();


// Add services to the container.
builder.Services.AddControllersWithViews();
//builder.Services.Configure<StripeSettings>(Configuration.GetSection())
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

// Creating a scope for dependency injection to ensure a scoped lifetime for services
using (var scope = app.Services.CreateScope())
{
    // Get the RoleManager service from the service provider (DI container)
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // Define the roles we want to ensure exist in the application
    var roles = new[] { "Admin", "Manager", "Staff" };

    // Loop through each role
    foreach (var role in roles)
    {
        // Check if the role already exists in the system
        if (!await roleManager.RoleExistsAsync(role))
        {
            // If the role does not exist, create it
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

// Creating another scope for dependency injection to manage user-related services
using (var scope = app.Services.CreateScope())
{
    // Get the UserManager service from the service provider (DI container)
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<TuckshopOrderingUser>>();

    // Define the details for the default admin user
    string email = "admin_tso@gmail.com";
    string password = "Admin123!";
    string firstName = "Will";
    string lastName = "Purdon";

    // Check if the admin user already exists in the system using the email
    if (await userManager.FindByEmailAsync(email) == null)
    {
        // If the user doesn't exist, create a new user object
        var user = new TuckshopOrderingUser
        {
            UserName = email, // Set the username to the email
            Email = email,    // Set the email
            firstName = firstName,  // Set the user's first name
            lastName = lastName     // Set the user's last name
        };

        // Create the user with the specified password
        await userManager.CreateAsync(user, password);

        // Assign the "Admin" role to the newly created user
        await userManager.AddToRoleAsync(user, "Admin");
    }
}


app.Run();
