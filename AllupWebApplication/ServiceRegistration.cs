using AllupWebApplication.Business.Interfaces;

namespace PustokMVC
{
    public static class ServiceRegistration
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<ISliderService, SliderService>();
        }
    }
}
