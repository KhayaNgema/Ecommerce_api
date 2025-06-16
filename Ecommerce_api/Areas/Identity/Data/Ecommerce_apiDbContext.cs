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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<StoreOwnerStore>()
            .HasKey(ss => new { ss.StoreOwnerId, ss.StoreId });

        modelBuilder.Entity<StoreOwnerStore>()
            .HasOne(ss => ss.StoreOwner)
            .WithMany(so => so.StoreOwnerStores)
            .HasForeignKey(ss => ss.StoreOwnerId);

        modelBuilder.Entity<StoreOwnerStore>()
            .HasOne(ss => ss.Store)
            .WithMany(s => s.StoreOwnerStores)
            .HasForeignKey(ss => ss.StoreId);

        modelBuilder.Entity<Store>()
            .HasOne(s => s.CreatedBy)
            .WithMany()
            .HasForeignKey(s => s.CreatedById)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Store>()
            .HasOne(s => s.ModifiedBy)
            .WithMany()
            .HasForeignKey(s => s.ModifiedById)
            .OnDelete(DeleteBehavior.NoAction);
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

    public DbSet<Ecommerce_api.Models.StoreOwnerStore> StoreOwnerStores { get; set; }

}
