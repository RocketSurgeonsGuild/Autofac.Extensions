using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Conventions.Autofac;

/// <summary>
/// Delegate ServiceConventionAction
/// </summary>
/// <param name="conventionContext"></param>
/// <param name="configuration"></param>
/// <param name="services"></param>
/// <param name="container"></param>
public delegate void AutofacConvention(IConventionContext conventionContext, IConfiguration configuration, IServiceCollection services, ContainerBuilder container);