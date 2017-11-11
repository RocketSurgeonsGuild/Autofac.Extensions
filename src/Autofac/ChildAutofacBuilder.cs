﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Builders;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Extensions.DependencyInjection;

namespace Rocket.Surgery.Extensions.Autofac
{
    public abstract class ChildAutofacBuilder : Builder<IAutofacBuilder>, IAutofacBuilder
    {
        protected readonly IAutofacBuilder Builder;

        protected ChildAutofacBuilder(IAutofacBuilder parent) : base(parent)
        {
            Builder = parent;
        }

        public IConfiguration Configuration => Builder.Configuration;
        public IServicesEnvironment Environment => Builder.Environment;
        public IAssemblyProvider AssemblyProvider => Builder.AssemblyProvider;
        public IAssemblyCandidateFinder AssemblyCandidateFinder => Builder.AssemblyCandidateFinder;
        public IServiceBuilderAndContainerWrapper System => Builder.System;
        public IServiceBuilderAndContainerWrapper Application => Builder.Application;
        public IAutofacBuilder AddDelegate(AutofacConventionDelegate @delegate) => Builder.AddDelegate(@delegate);
        public IAutofacBuilder AddConvention(IAutofacConvention convention) => Builder.AddConvention(convention);
        public IAutofacBuilder ConfigureContainer(ContainerBuilderDelegate builder)
        {
            return Builder.ConfigureContainer(builder);
        }
        public IServiceCollection Services => Builder.Services;
    }
}
