using Ecommerce_api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_api.Data;

public class Ecommerce_apiDBContext : IdentityDbContext<UserBaseModel>
{
    public Ecommerce_apiDBContext(DbContextOptions<Ecommerce_apiDBContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }

    public DbSet<Ecommerce_api.Models.RequestLog> RequestLogs { get; set; }

    public DbSet<Ecommerce_api.Models.Cart> Carts { get; set; }

    public DbSet<Ecommerce_api.Models.CartItem> CartItems { get; set; }

    public DbSet<Ecommerce_api.Models.ActivityLog> ActivityLogs { get; set; }

    public DbSet<Ecommerce_api.Models.Category> Categories { get; set; }

    public DbSet<Ecommerce_api.Models.Customer> Customers { get; set; }

    public DbSet<Ecommerce_api.Models.DeviceInfo> DeviceInfos { get; set; }

    public DbSet<Ecommerce_api.Models.Order> Orders { get; set; }

    public DbSet<Ecommerce_api.Models.OrderItem> OrderItems { get; set; }

    public DbSet<Ecommerce_api.Models.Product> Products{ get; set; }

    public DbSet<Ecommerce_api.Models.UserBaseModel> Users { get; set; }

    public DbSet<Ecommerce_api.Models.Store> Stores { get; set; }

    public DbSet<Ecommerce_api.Models.StoreOwner> StoreOwners { get; set; }

}
