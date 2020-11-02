using LtiAdvantage.IntegrationTests.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
#if NETCOREAPP3_1
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
#if NETCOREAPP2_1
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
#elif NETCOREAPP3_1
                .AddNewtonsoftJson();
#endif
            services.AddAuthentication()
                .AddScheme<TestAuthOptions, TestAuthHandler>(JwtBearerDefaults.AuthenticationScheme, options => { });

            services.AddLtiAdvantagePolicies();
        }

#if NETCOREAPP2_1
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStaticFiles();
            app.UseMvc();
        }
#elif NETCOREAPP3_1
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
