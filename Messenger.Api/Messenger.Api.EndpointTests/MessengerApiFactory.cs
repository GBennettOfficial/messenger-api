using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Messenger.Api.EndpointTests
{
    public class MessengerApiFactory : WebApplicationFactory<Program>
    {
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
