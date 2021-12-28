using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Conventions.Autofac;

/// <summary>
/// IAutofacConvention
/// Implements the <see cref="IConvention" />
/// </summary>
/// <seealso cref="IConvention" />
[PublicAPI]
public interface IAutofacConvention : IConvention
{
    /// <summary>
    /// Register additional things with the container
    /// </summary>
    /// <param name="conventionContext"></param>
    /// <param name="configuration"></param>
    /// <param name="services"></param>
    /// <param name="container"></param>
    void Register(IConventionContext conventionContext, IConfiguration configuration, IServiceCollection services, ContainerBuilder container);
}