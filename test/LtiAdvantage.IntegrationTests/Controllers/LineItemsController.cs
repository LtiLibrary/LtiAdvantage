using System.Threading.Tasks;
using LtiAdvantage.AspNetCore.AssignmentGradeServices;
using LtiAdvantage.AssignmentGradeServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
            // Echo the filters so tests can check how the query string was bound.
            Response.Headers["x-resource-link-id"] = request.ResourceLinkId ?? "";
            Response.Headers["x-resource-id"] = request.ResourceId ?? "";
            Response.Headers["x-tag"] = request.Tag ?? "";
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
