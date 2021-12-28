using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Conventions.Autofac;
/// <summary>
/// Autofac extension methods
/// </summary>
public static class AutofacExtensions
{
    /// <summary>
    /// Applies conventions to the given ContainerBuilder
    /// </summary>
    /// <param name="containerBuilder"></param>
    /// <param name="conventionContext"></param>
    /// <param name="services"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static ContainerBuilder ApplyConventions(this ContainerBuilder containerBuilder, IConventionContext conventionContext, IServiceCollection services)
    {
        var configuration = conventionContext.Get<IConfiguration>() ?? throw new ArgumentException("Configuration was not found in context");
        foreach (var item in conventionContext.Conventions.Get<IAutofacConvention, AutofacConvention>())
        {
            if (item is IAutofacConvention convention)
            {
                convention.Register(conventionContext, configuration, services, containerBuilder);
            }
            else if (item is AutofacConvention @delegate)
            {
                @delegate(conventionContext, configuration, services, containerBuilder);
            }
        }

        return containerBuilder;
    }
}
