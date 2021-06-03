using Microsoft.Extensions.DependencyInjection;

namespace Firenet
{
    public static class FireConfiguration
    {
        public static IServiceCollection AddFirenet<TContext>(this IServiceCollection services) where TContext : FireContext
        {
            var instance = FireContextBuilder<TContext>.Build();
            
            services.AddSingleton(instance);

            return services;
        }
    }
}
