using Microsoft.EntityFrameworkCore;
using OrderManagement.Application.DTOs;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Discount> Discounts { get; set; } = null!;
    public DbSet<OrderProduct> OrderProducts { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Name)
            .IsUnique();

        modelBuilder.Entity<Discount>()
            .HasOne(d => d.Product)
            .WithMany(p => p.Discounts)
            .HasForeignKey(d => d.ProductId)
            .IsRequired();

        modelBuilder.Entity<OrderProduct>()
            .HasOne(d => d.Product)
            .WithMany(p => p.OrderProducts)
            .HasForeignKey(d => d.ProductId)
            .IsRequired();

        modelBuilder.Entity<OrderProduct>()
            .HasOne(d => d.Order)
            .WithMany(p => p.Products)
            .HasForeignKey(d => d.OrderId)
            .IsRequired();

        modelBuilder.Entity<Order>()
            .HasIndex(o => o.Number)
            .IsUnique();
        modelBuilder.Entity<Order>()
            .Property(o => o.Number)
            .ValueGeneratedOnAdd();


        base.OnModelCreating(modelBuilder);
    }
}