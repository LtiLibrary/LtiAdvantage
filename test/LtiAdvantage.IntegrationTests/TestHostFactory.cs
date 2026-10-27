using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LtiAdvantage.IntegrationTests
{
    /// <summary>
    /// Builds and starts an in-memory test host running <see cref="Startup"/>.
    /// Use <c>host.GetTestClient()</c> to send requests, and dispose the host when done.
    /// </summary>
    public static class TestHostFactory
    {
        public static IHost Create()
        {
            return new HostBuilder()
                .ConfigureWebHost(webHost => webHost
                    .UseTestServer()
                    .UseContentRoot(AppContext.BaseDirectory)
                    .UseStartup<Startup>())
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                    logging.AddDebug();
                })
                .Start();
        }
    }
}
