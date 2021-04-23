using Microsoft.Extensions.DependencyInjection;
using System;

namespace OrdersApi.AutoMappers
{
    public static class AutoMapperConfiguration
    {
        public static void AddAutoMapperConfiguration(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            services.AddAutoMapper(typeof(EntityToResultMappingProfile), typeof(EntityToDtoMappingProfile));
        }
    }
}
