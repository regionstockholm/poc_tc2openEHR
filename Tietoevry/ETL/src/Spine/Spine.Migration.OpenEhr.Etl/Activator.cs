using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spine.Migration.OpenEhr.Etl.Core;

namespace Spine.Migration.OpenEhr.Etl
{
    public class Activation
    {
        private IConfiguration _configuration = null!;
        private IServiceProvider _serviceProvider = null!;
        private readonly IServiceCollection _services;

        private Type _registeredEtlType = null!;

        public static Activation Instance { get; } = new();

        private Activation()
        {
            _services = new ServiceCollection();
        }

        private class LazyService<T> : Lazy<T>
        {
            public LazyService(IServiceProvider serviceProvider)
                : base(() => serviceProvider.GetRequiredService<T>())
            {
            }
        }

        public Activation Configure(string[] args)
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ENVIRONMENT__NAME")}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args).Build();

            return this;
        }

        public Activation RegisterServices()
        {
            _services.AddSingleton<IConfiguration>(_configuration);

            _services.AddTransient(typeof(Lazy<>), typeof(LazyService<>));
            _services.AddBootStrapper();
            _services.AddOpenEhrClient(_configuration);

            return this;
        }
        public Activation RegisterEtl<EtlHandler>() where EtlHandler : class, IEtlHandler
        {
            _registeredEtlType = typeof(EtlHandler);
            _services.AddSingleton<EtlHandler>();
            return this;
        }
        public Activation RegisterService<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            _services.AddSingleton<TService, TImplementation>();
            return this;
        }
        public Activation ConfigureServics(Action<IServiceCollection> actionSevices)
        {
            actionSevices?.Invoke(_services);
            return this;
        }


        public Activation Build()
        {
            _serviceProvider = _services.BuildServiceProvider();
            return this;
        }

        public void Run()
        {
            if (_registeredEtlType == null)
            {
                throw new InvalidOperationException($"EtlHandler is not registered. {Environment.NewLine} Register using {nameof(RegisterEtl)} method.");
            }

            var currentEtlHandler = _serviceProvider.GetRequiredService(_registeredEtlType) as IEtlHandler
                ?? throw new InvalidCastException($"Provided type of {_registeredEtlType} is not registered.");
            currentEtlHandler.Execute();
        }
    }
}
