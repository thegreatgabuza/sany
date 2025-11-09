using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cascade.Areas.Identity.Pages.Account
{
    public class NotFoundModel : PageModel
    {
        private readonly ILogger<NotFoundModel> _logger;

        public NotFoundModel(ILogger<NotFoundModel> logger)
        {
            _logger = logger;
        }

        public string? RequestedUrl { get; set; }
        public string? ReturnUrl { get; set; }

        public void OnGet(string? returnUrl = null)
        {
            RequestedUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
            ReturnUrl = returnUrl;
            
            _logger.LogWarning("404 Error: Page not found. Requested URL: {RequestedUrl}, Return URL: {ReturnUrl}", 
                RequestedUrl, ReturnUrl);
        }
    }
}
