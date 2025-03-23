using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InventorySystemWebUI.Pages
{
    public class IndexModel : PageModel
    {
        public ActionResult OnGet()
        {
            return Page();
        }
    }
}
