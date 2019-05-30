﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autofac;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.Autofac.Tests;
using Rocket.Surgery.Extensions.DependencyInjection;
using Rocket.Surgery.Extensions.Testing;
using Xunit;
using Xunit.Abstractions;

[assembly: Convention(typeof(AutofacBuilderTests.AbcConvention))]
[assembly: Convention(typeof(AutofacBuilderTests.OtherConvention))]

namespace Rocket.Surgery.Extensions.Autofac.Tests
{
    public class AutofacBuilderTests : AutoTestBase
    {
        public AutofacBuilderTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            AutoFake.Provide<DiagnosticSource>(new DiagnosticListener("Test"));
        }

        [Fact]
        public void Constructs()
        {
            var assemblytProvider = AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            var services = AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            AutoFake.Provide(new ContainerBuilder());
            var servicesBuilder = AutoFake.Resolve<AutofacBuilder>();

            servicesBuilder.AssemblyProvider.Should().BeSameAs(assemblytProvider);
            servicesBuilder.AssemblyCandidateFinder.Should().NotBeNull();
            servicesBuilder.Services.Should().BeSameAs(services);
            servicesBuilder.Configuration.Should().NotBeNull();
            servicesBuilder.Environment.Should().NotBeNull();

            Action a = () => { servicesBuilder.PrependConvention(A.Fake<IAutofacConvention>()); };
            a.Should().NotThrow();
            a = () => { servicesBuilder.PrependDelegate(delegate { }); };
            a.Should().NotThrow();
            a = () => { servicesBuilder.ConfigureContainer(delegate { }); };
            a.Should().NotThrow();
        }

        [Fact]
        public void StoresAndReturnsItems()
        {
            AutoFake.Provide<IDictionary<object, object>>(new Dictionary<object, object>());
            var servicesBuilder = AutoFake.Resolve<AutofacBuilder>();

            var value = new object();
            servicesBuilder[string.Empty] = value;
            servicesBuilder[string.Empty].Should().BeSameAs(value);
        }

        [Fact]
        public void IgnoreNonExistentItems()
        {
            AutoFake.Provide<IDictionary<object, object>>(new Dictionary<object, object>());
            var servicesBuilder = AutoFake.Resolve<AutofacBuilder>();

            servicesBuilder[string.Empty].Should().BeNull();
        }

        [Fact]
        public void AddConventions()
        {
            var servicesBuilder = AutoFake.Resolve<AutofacBuilder>();

            var convention = A.Fake<IAutofacConvention>();

            servicesBuilder.PrependConvention(convention);

            A.CallTo(() => AutoFake.Resolve<IConventionScanner>().PrependConvention(convention)).MustHaveHappened();
        }

        public interface IAbc { }
        public interface IAbc2 { }
        public interface IAbc3 { }
        public interface IAbc4 { }
        public interface IOtherAbc3 { }
        public interface IOtherAbc4 { }

        public class AbcConvention : IAutofacConvention
        {
            public void Register(IAutofacConventionContext context)
            {
                context.ConfigureContainer(c => c.RegisterInstance(A.Fake<IAbc>()));
                context.Services.AddSingleton(A.Fake<IAbc2>());
                context.ConfigureContainer(c => { });
            }
        }

        public class OtherConvention : IServiceConvention
        {
            public void Register(IServiceConventionContext context)
            {
                context.Services.AddSingleton(A.Fake<IOtherAbc3>());
            }
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithCore()
        {
            AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            AutoFake.Provide(new ContainerBuilder());
            var servicesBuilder = AutoFake.Resolve<AutofacBuilder>();

            servicesBuilder.ConfigureContainer(c => c.RegisterInstance(A.Fake<IAbc>()));
            servicesBuilder.Services.AddSingleton(A.Fake<IAbc2>());

            var items = servicesBuilder.Build();
            items.ResolveOptional<IAbc>().Should().NotBeNull();
            items.ResolveOptional<IAbc2>().Should().NotBeNull();
            items.ResolveOptional<IAbc3>().Should().BeNull();
            items.ResolveOptional<IAbc4>().Should().BeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithApplication()
        {
            AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            AutoFake.Provide(new ContainerBuilder());
            var servicesBuilder = AutoFake.Resolve<AutofacBuilder>();

            servicesBuilder.ConfigureContainer(c => c.RegisterInstance(A.Fake<IAbc>()));
            servicesBuilder.Services.AddSingleton(A.Fake<IAbc2>());
            servicesBuilder.ConfigureContainer(c => c.RegisterInstance(A.Fake<IAbc4>()));

            var items = servicesBuilder.Build();
            items.ResolveOptional<IAbc>().Should().NotBeNull();
            items.ResolveOptional<IAbc2>().Should().NotBeNull();
            items.ResolveOptional<IAbc3>().Should().BeNull();
            items.ResolveOptional<IAbc4>().Should().NotBeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithSystem()
        {
            AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            AutoFake.Provide(new ContainerBuilder());
            var servicesBuilder = AutoFake.Resolve<AutofacBuilder>();

            servicesBuilder.ConfigureContainer(c => c.RegisterInstance(A.Fake<IAbc3>()));
            servicesBuilder.ConfigureContainer(c => c.RegisterInstance(A.Fake<IAbc4>()));

            var items = servicesBuilder.Build();
            items.ResolveOptional<IAbc>().Should().BeNull();
            items.ResolveOptional<IAbc2>().Should().BeNull();
            items.ResolveOptional<IAbc3>().Should().NotBeNull();
            items.ResolveOptional<IAbc4>().Should().NotBeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithCore_ServiceProvider()
        {
            AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            AutoFake.Provide(new ContainerBuilder());
            var servicesBuilder = AutoFake.Resolve<AutofacBuilder>();

            servicesBuilder.ConfigureContainer(c => c.RegisterInstance(A.Fake<IAbc>()));
            servicesBuilder.Services.AddSingleton(A.Fake<IAbc2>());

            var items = servicesBuilder.Build();

            var sp = items.Resolve<IServiceProvider>();
            sp.GetService<IAbc>().Should().NotBeNull();
            sp.GetService<IAbc2>().Should().NotBeNull();
            sp.GetService<IAbc3>().Should().BeNull();
            sp.GetService<IAbc4>().Should().BeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithApplication_ServiceProvider()
        {
            AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            AutoFake.Provide(new ContainerBuilder());
            var servicesBuilder = AutoFake.Resolve<AutofacBuilder>();

            servicesBuilder.ConfigureContainer(c => c.RegisterInstance(A.Fake<IAbc>()));
            servicesBuilder.Services.AddSingleton(A.Fake<IAbc2>());
            servicesBuilder.ConfigureContainer(c => c.RegisterInstance(A.Fake<IAbc4>()));

            var items = servicesBuilder.Build();
            var sp = items.Resolve<IServiceProvider>();
            sp.GetService<IAbc>().Should().NotBeNull();
            sp.GetService<IAbc2>().Should().NotBeNull();
            sp.GetService<IAbc3>().Should().BeNull();
            sp.GetService<IAbc4>().Should().NotBeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithSystem_ServiceProvider()
        {
            var servicesBuilder = AutoFake.Resolve<AutofacBuilder>();

            servicesBuilder.ConfigureContainer(c => c.RegisterInstance(A.Fake<IAbc3>()));
            servicesBuilder.ConfigureContainer(c => c.RegisterInstance(A.Fake<IAbc4>()));

            var items = servicesBuilder.Build();
            var sp = items.Resolve<IServiceProvider>();
            sp.GetService<IAbc>().Should().BeNull();
            sp.GetService<IAbc2>().Should().BeNull();
            sp.GetService<IAbc3>().Should().NotBeNull();
            sp.GetService<IAbc4>().Should().NotBeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithSystem_UsingConvention()
        {
            var assemblyProvider = AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IConventionScanner>(AutoFake.Resolve<AggregateConventionScanner>());
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            AutoFake.Provide(new ContainerBuilder());
            var servicesBuilder = AutoFake.Resolve<AutofacBuilder>();

            A.CallTo(() => AutoFake.Resolve<IAssemblyCandidateFinder>().GetCandidateAssemblies(A<IEnumerable<string>>._))
                .Returns(assemblyProvider.GetAssemblies());

            var items = servicesBuilder.Build();
            items.ResolveOptional<IAbc>().Should().NotBeNull();
            items.ResolveOptional<IAbc2>().Should().NotBeNull();
            items.ResolveOptional<IAbc3>().Should().BeNull();
            items.ResolveOptional<IAbc4>().Should().BeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithSystem_UsingConvention_IncludingOtherBits()
        {
            var assemblyProvider = AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IConventionScanner>(AutoFake.Resolve<AggregateConventionScanner>());
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            AutoFake.Provide(new ContainerBuilder());
            var servicesBuilder = AutoFake.Resolve<AutofacBuilder>();

            A.CallTo(() => AutoFake.Resolve<IAssemblyCandidateFinder>().GetCandidateAssemblies(A<IEnumerable<string>>._))
                .Returns(assemblyProvider.GetAssemblies());

            var items = servicesBuilder.Build();
            items.ResolveOptional<IAbc>().Should().NotBeNull();
            items.ResolveOptional<IAbc2>().Should().NotBeNull();
            items.ResolveOptional<IAbc3>().Should().BeNull();
            items.ResolveOptional<IAbc4>().Should().BeNull();
            items.ResolveOptional<IOtherAbc3>().Should().NotBeNull();
            items.ResolveOptional<IOtherAbc3>().Should().NotBeNull();
        }

        [Fact]
        public void SendsNotificationThrough_OnBuild_Observable_ForMicrosoftExtensions()
        {
            var assemblyProvider = AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            AutoFake.Provide(new ContainerBuilder());
            var servicesBuilder = AutoFake.Resolve<AutofacBuilder>();

            A.CallTo(() => AutoFake.Resolve<IAssemblyCandidateFinder>().GetCandidateAssemblies(A<IEnumerable<string>>._))
                .Returns(assemblyProvider.GetAssemblies());

            var observer = A.Fake<IObserver<IServiceProvider>>();
            var observerApplication = A.Fake<IObserver<IServiceProvider>>();
            var observerSystem = A.Fake<IObserver<IServiceProvider>>();
            ((IServiceConventionContext)servicesBuilder).OnBuild.Subscribe(observer);

            var items = servicesBuilder.Build();

            A.CallTo(() => observer.OnNext(A<IServiceProvider>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void SendsNotificationThrough_OnBuild_Observable_ForAutofac()
        {
            var assemblyProvider = AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            AutoFake.Provide(new ContainerBuilder());
            var servicesBuilder = AutoFake.Resolve<AutofacBuilder>();

            A.CallTo(() => AutoFake.Resolve<IAssemblyCandidateFinder>().GetCandidateAssemblies(A<IEnumerable<string>>._))
                .Returns(assemblyProvider.GetAssemblies());

            var observer = A.Fake<IObserver<IServiceProvider>>();
            var observerContainer = A.Fake<IObserver<IContainer>>();
            var observerApplication = A.Fake<IObserver<ILifetimeScope>>();
            var observerSystem = A.Fake<IObserver<ILifetimeScope>>();
            servicesBuilder.OnContainerBuild.Subscribe(observerContainer);
            servicesBuilder.OnBuild.Subscribe(observer);

            var container = servicesBuilder.Build();

            A.CallTo(() => observer.OnNext(A.Fake<IServiceProvider>())).MustHaveHappenedOnceExactly();
            A.CallTo(() => observerContainer.OnNext(container)).MustHaveHappenedOnceExactly();
        }
    }
}
