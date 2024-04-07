using AllupWebApplication.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AllupWebApplication.Data;

public class AllupDbContext : IdentityDbContext<AppUser>
{
    public AllupDbContext(DbContextOptions<AllupDbContext> options) : base(options) { }
    public DbSet<SliderItem> SliderItems { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<AppUser> AppUsers { get; set; }
}
