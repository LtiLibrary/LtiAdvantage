using LtiAdvantage.IntegrationTests.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
#if NET6_0
using Microsoft.Extensions.Hosting;
#endif

namespace LtiAdvantage.IntegrationTests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddApplicationPart(typeof(LineItemsController).Assembly)
#if NET6_0
                .AddNewtonsoftJson();
#else
                ;
#endif
            services.AddAuthentication()
                .AddScheme<TestAuthOptions, TestAuthHandler>(JwtBearerDefaults.AuthenticationScheme, options => { });

            services.AddLtiAdvantagePolicies();
        }

#if NET6_0_OR_GREATER
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller}/{action=Index}/{id?}");
            });
        }
#endif
    }
}
