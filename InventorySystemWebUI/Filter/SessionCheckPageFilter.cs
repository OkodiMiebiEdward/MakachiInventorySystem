using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InventorySystemWebUI.Filter
{
    public class SessionCheckPageFilter : IPageFilter
    {
        private const int TokenExpirySeconds = 600;

        public void OnPageHandlerSelected(PageHandlerSelectedContext context) { }

        public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            var path = context.HttpContext.Request.Path;

            // Skip certain pages
            var skipPaths = new[] { "/", "/Index", "/Login", "/SessionExpired" };
            if (skipPaths.Contains(path.ToString()))
                return;

            var token = context.HttpContext.Session.GetString("AuthToken");
            var issuedAtTicksStr = context.HttpContext.Session.GetString("AuthTokenIssuedAt");

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(issuedAtTicksStr))
            {
                context.Result = new RedirectToPageResult("/SessionExpired");
                return;
            }

            if (long.TryParse(issuedAtTicksStr, out var issuedAtTicks))
            {
                var issuedAt = new DateTime(issuedAtTicks, DateTimeKind.Utc);
                var now = DateTime.UtcNow;
                var elapsedSeconds = (now - issuedAt).TotalSeconds;

                if (elapsedSeconds > TokenExpirySeconds)
                {
                    context.Result = new RedirectToPageResult("/SessionExpired");
                    return;
                }
            }
            else
            {
                // Invalid timestamp format
                context.Result = new RedirectToPageResult("/SessionExpired");
                return;
            }
        }

        public void OnPageHandlerExecuted(PageHandlerExecutedContext context) { }
    }


}
