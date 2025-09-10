using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthWebAPI.API.Controllers;
using AuthWebAPI.Application.Interfaces;
using AuthWebAPI.Infrastructure.Services;
using AuthWebAPI.Persistance.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthWebAPI.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureDI(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<WebAPIDbContext>(opt =>
            {
                opt.UseSqlServer(configuration.GetConnectionString("API_DB"));
            });

            //Dependency Injection (Life time)
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IChatService, ChatService>();

            return services;
        }
    }
}
