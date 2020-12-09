using System.Threading.Tasks;
using LtiAdvantage.AspNetCore.AssignmentGradeServices;
using LtiAdvantage.AssignmentGradeServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
#if NETCOREAPP2_1
using IWebHostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
#endif

namespace LtiAdvantage.IntegrationTests.Controllers
{
    public class LineItemsController : LineItemsControllerBase
    {
        public LineItemsController(IWebHostEnvironment env, ILogger<LineItemsControllerBase> logger) : base(env, logger)
        {
        }

        protected override Task<ActionResult<LineItem>> OnAddLineItemAsync(AddLineItemRequest request)
        {
            var result = (ActionResult<LineItem>) new ObjectResult(request.LineItem);
            return Task.FromResult(result);
        }

        protected override Task<ActionResult> OnDeleteLineItemAsync(DeleteLineItemRequest request)
        {
            var result = (ActionResult) Ok();
            return Task.FromResult(result);
        }

        protected override Task<ActionResult<LineItem>> OnGetLineItemAsync(GetLineItemRequest request)
        {
            var result = (ActionResult<LineItem>) new ObjectResult(new LineItem());
            return Task.FromResult(result);
        }

        protected override Task<ActionResult<LineItemContainer>> OnGetLineItemsAsync(GetLineItemsRequest request)
        {
            var result = (ActionResult<LineItemContainer>) new ObjectResult(new LineItemContainer());
            return Task.FromResult(result);
        }

        protected override Task<ActionResult> OnUpdateLineItemAsync(UpdateLineItemRequest request)
        {
            var result = (ActionResult) Ok();
            return Task.FromResult(result);
        }
    }
}
