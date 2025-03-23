using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InventorySystemWebUI.Pages;

public class HomeModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public HomeModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }
}
