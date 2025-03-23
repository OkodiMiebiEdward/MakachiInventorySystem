using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InventorySystemWebUI.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        public ActionResult OnGet()
        {
            return Page();
        }
    }
}
