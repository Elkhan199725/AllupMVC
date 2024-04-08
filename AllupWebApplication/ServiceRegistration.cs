using AllupWebApplication.Business.Implementations;
using AllupWebApplication.Business.Interfaces;

namespace AllupMVC;

public static class ServiceRegistration
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<ISliderService, SliderService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();

    }
}
