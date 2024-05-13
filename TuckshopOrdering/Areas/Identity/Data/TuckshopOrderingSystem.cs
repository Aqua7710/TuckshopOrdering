using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TuckshopOrdering.Areas.Identity.Data;
using TuckshopOrdering.Models;

namespace TuckshopOrdering.Areas.Identity.Data;

public class TuckshopOrderingSystem : IdentityDbContext<TuckshopOrderingUser>
{
    public TuckshopOrderingSystem(DbContextOptions<TuckshopOrderingSystem> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }

    public DbSet<TuckshopOrdering.Models.Menu> Menu { get; set; } = default!;

    public DbSet<TuckshopOrdering.Models.Order> Order { get; set; } = default!;

    public DbSet<TuckshopOrdering.Models.Category> Category { get; set; } = default!;

    public DbSet<TuckshopOrdering.Models.Customise> Customise { get; set; } = default!;

    public DbSet<TuckshopOrdering.Models.Customer> Customer { get; set; } = default!;

    public DbSet<TuckshopOrdering.Models.FoodOrder> FoodOrder { get; set; } = default!;
}
