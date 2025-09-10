using AuthWebAPI.Application;
using AuthWebAPI.Domain;
using AuthWebAPI.Infrastructure;
using AuthWebAPI.Persistance;

namespace AuthWebAPI.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWebAPIDI(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApplicationDI()
                .AddInfrastructureDI(configuration).AddDomainDI();
            return services;
            //return services.AddSingleton<IServiceCollection>();
        }
    }
}
