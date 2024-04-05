using AllupWebApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace AllupWebApplication.Data;

public class AllupDbContext : DbContext
{
    public AllupDbContext(DbContextOptions<AllupDbContext> options) : base(options) { }
    public DbSet<SliderItem> SliderItems { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
}
