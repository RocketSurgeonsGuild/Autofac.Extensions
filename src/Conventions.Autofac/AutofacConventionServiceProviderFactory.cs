using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Conventions.Autofac;

class AutofacConventionServiceProviderFactory : IServiceProviderFactory<ContainerBuilder>
{
    private readonly IConventionContext _conventionContext;
    private readonly ContainerBuilder _container;

    public AutofacConventionServiceProviderFactory(IConventionContext conventionContext, ContainerBuilder? container = null)
    {
        _conventionContext = conventionContext;
        _container = container ?? new ContainerBuilder();
    }

    public ContainerBuilder CreateBuilder(IServiceCollection services)
    {
        _container.ApplyConventions(_conventionContext, services);
        _container.Populate(services);
        return _container;
    }

    public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder) => containerBuilder.Build().Resolve<IServiceProvider>();
}
