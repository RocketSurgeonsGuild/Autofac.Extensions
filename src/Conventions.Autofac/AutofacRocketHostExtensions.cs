using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions.Autofac;

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Conventions;

/// <summary>
/// Class AutofacRocketHostExtensions.
/// </summary>
[PublicAPI]
public static class AutofacConventionRocketHostExtensions
{
    /// <summary>
    /// Uses the Autofac.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="delegate">The container.</param>
    /// <returns>IHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureAutofac(this ConventionContextBuilder builder, AutofacConvention @delegate)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.AppendDelegate(@delegate);
        return builder;
    }

    /// <summary>
    /// Uses the Autofac.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="delegate">The container.</param>
    /// <returns>IHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureAutofac(
        this ConventionContextBuilder builder,
        Action<IConfiguration, IServiceCollection, ContainerBuilder> @delegate
    )
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.AppendDelegate(new AutofacConvention((context, configuration, services, container) => @delegate(configuration, services, container)));
        return builder;
    }

    /// <summary>
    /// Uses the Autofac.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="delegate">The container.</param>
    /// <returns>IHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureAutofac(this ConventionContextBuilder builder, Action<IServiceCollection, ContainerBuilder> @delegate)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.AppendDelegate(new AutofacConvention((context, configuration, services, container) => @delegate(services, container)));
        return builder;
    }

    /// <summary>
    /// Uses the Autofac.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="delegate">The container.</param>
    /// <returns>IHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureAutofac(this ConventionContextBuilder builder, Action<ContainerBuilder> @delegate)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.AppendDelegate(new AutofacConvention((context, configuration, services, container) => @delegate(container)));
        return builder;
    }
}
