using AllupWebApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace AllupWebApplication.Data;

public class AllupDbContext : DbContext
{
    public AllupDbContext(DbContextOptions<AllupDbContext> options) : base(options) { }
    public DbSet<SliderItem> SliderItems { get; set; }
}
