using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LtiAdvantage.IntegrationTests
{
    /// <summary>
    /// Builds and starts an in-memory test host running <see cref="Startup"/>.
    /// Use <c>host.GetTestClient()</c> to send requests, and dispose the host when done.
    /// <c>configureServices</c> adds to (or overrides) the services <see cref="Startup"/> registers.
    /// </summary>
    public static class TestHostFactory
    {
        public static IHost Create(Action<IServiceCollection> configureServices = null)
        {
            return new HostBuilder()
                .ConfigureWebHost(webHost => webHost
                    .UseTestServer()
                    .UseContentRoot(AppContext.BaseDirectory)
                    .UseStartup<Startup>()
                    .ConfigureServices(services => configureServices?.Invoke(services)))
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                    logging.AddDebug();
                })
                .Start();
        }
    }
}
