using System;
using Autofac;
using Autofac.Core;
using Moq;

namespace Messerli.Test.Utility
{
    public class ModuleBuilder
    {
        private readonly IModule _module;
        private readonly ContainerBuilder _builder = new ContainerBuilder();

        public ModuleBuilder(IModule module)
        {
            _module = module;
        }

        public ModuleBuilder RegisterMockFor<TInterface>() where TInterface : class
        {
            _builder.Register(context => new Mock<TInterface>().Object).As<TInterface>();

            return this;
        }

        public ModuleBuilder MockEnvironmentVariable(string variable, string value)
        {
            Environment.SetEnvironmentVariable(variable, value);

            return this;
        }

        public IContainer Build()
        {
            _builder.RegisterModule(_module);

            return _builder.Build();
        }
    }
}
