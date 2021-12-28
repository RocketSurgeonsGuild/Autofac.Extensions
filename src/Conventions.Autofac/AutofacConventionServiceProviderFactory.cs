using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
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
        var container = _container;

        var configuration = _conventionContext.Get<IConfiguration>() ?? throw new ArgumentException("Configuration was not found in context");
        foreach (var item in _conventionContext.Conventions.Get<IAutofacConvention, AutofacConvention>())
        {
            if (item is IAutofacConvention convention)
            {
                convention.Register(_conventionContext, configuration, services, _container);
            }
            else if (item is AutofacConvention @delegate)
            {
                @delegate(_conventionContext, configuration, services, _container);
            }
        }

        container.Populate(services);

        return container;
    }

    public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder) => containerBuilder.Build().Resolve<IServiceProvider>();
}