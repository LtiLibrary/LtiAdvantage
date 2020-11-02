using System.Threading.Tasks;
using LtiAdvantage.NamesRoleProvisioningService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
#if NETCOREAPP2_1
using IWebHostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
#endif

namespace LtiAdvantage.IntegrationTests.Controllers
{
    public class MembershipController : MembershipControllerBase
    {
        public MembershipController(IWebHostEnvironment env, ILogger<MembershipControllerBase> logger) : base(env, logger)
        {
        }

        protected override Task<ActionResult<MembershipContainer>> OnGetMembershipAsync(GetMembershipRequest request)
        {
            var result = (ActionResult<MembershipContainer>) new ObjectResult(new MembershipContainer());
            return Task.FromResult(result);
        }
    }
}
