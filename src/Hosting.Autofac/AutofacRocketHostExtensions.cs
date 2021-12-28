using Autofac;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Autofac;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Hosting;

/// <summary>
/// Class AutofacRocketHostExtensions.
/// </summary>
[PublicAPI]
public static class AutofacRocketHostExtensions
{
    /// <summary>
    /// Uses the autofac.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="containerBuilder">The container builder.</param>
    /// <returns>ConventionContextBuilder.</returns>
    public static ConventionContextBuilder UseAutofac(this ConventionContextBuilder builder, ContainerBuilder? containerBuilder = null)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.ConfigureHosting((context, builder) => builder.UseServiceProviderFactory(new AutofacConventionServiceProviderFactory(context, containerBuilder)));
    }
}
