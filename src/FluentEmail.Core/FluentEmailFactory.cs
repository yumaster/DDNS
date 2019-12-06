using Microsoft.Extensions.DependencyInjection;
using System;

namespace FluentEmail.Core
{
    /// <summary>
    /// 流利的电子邮件工厂
    /// </summary>
    public class FluentEmailFactory : IFluentEmailFactory
    {
        private IServiceProvider services;

        public FluentEmailFactory(IServiceProvider services) => this.services = services;

        public IFluentEmail Create() => services.GetService<IFluentEmail>();
    }
}
