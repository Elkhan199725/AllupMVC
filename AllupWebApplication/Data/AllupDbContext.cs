using AllupWebApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace AllupWebApplication.Data;

public class AllupDbContext : DbContext
{
    public DbSet<SliderItem> SliderItems { get; set; }
}
