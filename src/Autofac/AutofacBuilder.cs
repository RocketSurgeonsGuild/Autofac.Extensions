﻿using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.DependencyInjection;

namespace Rocket.Surgery.Extensions.Autofac
{
    public class AutofacBuilder : AutofacBuilderBase
    {
        public AutofacBuilder(
            IAssemblyProvider assemblyProvider,
            IAssemblyCandidateFinder assemblyCandidateFinder,
            IConventionScanner scanner,
            IServiceCollection services,
            IConfiguration configuration,
            IServicesEnvironment environment) :
            base(assemblyProvider, assemblyCandidateFinder, scanner, services, configuration, environment){}

        /// <summary>
        /// Builds the root container, and returns the lifetime scopes for the application and system containers
        /// </summary>
        /// <param name="containerBuilder"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public IContainer Build(ContainerBuilder containerBuilder, ILogger logger)
        {
            Composer.Register<IAutofacConventionContext, IAutofacConvention, AutofacConventionDelegate>(_scanner, logger, this);

            _core.Collection.Apply(containerBuilder);
            containerBuilder.Populate(Services);

            _application.Collection.Apply(containerBuilder);
            containerBuilder.Populate(_application.Services);

            return containerBuilder.Build();
        }
    }
}
