using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace FirstWebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            // un structured logging
            _logger.LogInformation("Information");
            _logger.LogError("Error");
            _logger.LogDebug("Debug");
            _logger.LogWarning("warning");

            //structured logging

            var req = new
            {
                name = "Krishna",
                lname = "Sarigopula"
            };

            _logger.LogInformation("request info: {0}", req);
        }
    }
}