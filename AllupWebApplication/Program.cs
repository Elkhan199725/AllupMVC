using AllupMVC;
using AllupWebApplication;
using AllupWebApplication.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure Entity Framework Core with SQL Server
builder.Services.AddDbContext<AllupDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("default"));
});

// Method extension to encapsulate service registrations
builder.Services.AddServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // Use exception handler for production environment
    app.UseExceptionHandler("/Home/Error");

    // Enforce strict transport security
    app.UseHsts();
}

// Use HTTPS redirection
app.UseHttpsRedirection();

// Serve static files (e.g., JavaScript, CSS, images)
app.UseStaticFiles();

// Enable routing
app.UseRouting();

// Enable authentication/authorization middleware
app.UseAuthorization();

// Define routing for areas
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Define default routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Start the application
app.Run();
