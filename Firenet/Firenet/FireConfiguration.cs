using Microsoft.Extensions.DependencyInjection;
using System;

namespace Firenet
{
    public static class FireConfiguration
    {
        /// <summary>
        /// Build and inject a singleton <typeparamref name="TContext"/> with the options.
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddFirenet<TContext>(this IServiceCollection services, Action<FireOption> options)
            where TContext : FireContext
        {
            try
            {
                var instance = Activator.CreateInstance(typeof(TContext), options) as TContext;
                services.AddSingleton(instance);
                return services;
            }
            catch (Exception e)
            {
                throw e.InnerException;
            }
        }

        /// <summary>
        /// Build and inject a singleton <typeparamref name="TContext"/> with the environment variable 
        /// 'GOOGLE_APPLICATION_CREDENTIALS' which should previusly configured.
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddFirenet<TContext>(this IServiceCollection services)
            where TContext : FireContext
        {
            try
            {
                var instance = Activator.CreateInstance<TContext>();
                services.AddSingleton(instance);
                return services;
            }
            catch (Exception e)
            {
                throw e.InnerException;
            }
        }
    }
}
