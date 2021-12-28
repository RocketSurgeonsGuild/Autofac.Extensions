using System.Reflection;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions.Autofac;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Extensions.Autofac.Tests;

internal class TestAssemblyProvider : IAssemblyProvider
{
    public IEnumerable<Assembly> GetAssemblies() => new[]
    {
        typeof(AutofacConventionServiceProviderFactory).GetTypeInfo().Assembly,
        typeof(AutofacRocketHostExtensions).GetTypeInfo().Assembly,
        typeof(WebAssemblyAutofacRocketHostExtensions).GetTypeInfo().Assembly,
        typeof(TestAssemblyProvider).GetTypeInfo().Assembly
    };
}