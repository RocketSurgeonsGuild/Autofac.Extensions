using System;
using Autofac;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Conventions.Autofac
{
    /// <summary>
    /// IAutofacConventionContext
    /// Implements the <see cref="IConventionContext" />
    /// </summary>
    /// <seealso cref="IConventionContext" />
    [PublicAPI]
    public interface IAutofacConventionContext : IConventionContext
    {
        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        [NotNull] IConfiguration Configuration { get; }

        /// <summary>
        /// Gets the assembly provider.
        /// </summary>
        /// <value>The assembly provider.</value>
        [NotNull] IAssemblyProvider AssemblyProvider { get; }

        /// <summary>
        /// Gets the assembly candidate finder.
        /// </summary>
        /// <value>The assembly candidate finder.</value>
        [NotNull] IAssemblyCandidateFinder AssemblyCandidateFinder { get; }

        /// <summary>
        /// Gets the services.
        /// </summary>
        /// <value>The services.</value>
        [NotNull] IServiceCollection Services { get; }

        /// <summary>
        /// Gets the on container build.
        /// </summary>
        /// <value>The on container build.</value>
        [NotNull] IObservable<IContainer> OnContainerBuild { get; }

        /// <summary>
        /// The environment that this convention is running
        /// Based on IHostEnvironment / IHostingEnvironment
        /// </summary>
        /// <value>The environment.</value>
        [NotNull] IHostEnvironment Environment { get; }

        /// <summary>
        /// Configures the container.
        /// </summary>
        /// <param name="builder">The builder.</param>
        void ConfigureContainer([NotNull] ContainerBuilderDelegate builder);
    }
}