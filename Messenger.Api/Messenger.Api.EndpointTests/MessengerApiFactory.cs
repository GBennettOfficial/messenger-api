using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.Api.EndpointTests
{
    public class MessengerApiFactory : WebApplicationFactory<Program>
    {
        private IServiceProvider? _serviceProvider = null;

        internal T Inject<T>()
        {
            if (_serviceProvider is null)
            {
                var scope = Services.CreateScope();
                _serviceProvider = scope.ServiceProvider;
            }
            return (T)_serviceProvider.GetService(typeof(T))!;
        }

        internal string GetConnectionString(string name)
        {
            IConfiguration configuration = (IConfiguration)Services.GetService(typeof(IConfiguration))!;
            return configuration.GetConnectionString(name)!;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
        }
    }
}
