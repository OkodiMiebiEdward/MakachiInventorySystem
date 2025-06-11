using InventorySystemWebUI.Models;
using InventorySystemWebUI.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InventorySystemWebUI.Pages.Module.Product
{
    
    public class CreateProductModel : PageModel
    {
        public ResponseModel ResponseModel { get; set; }

        [BindProperty]
        public ProductVM Product { get; set; }
        public void OnGet()
        {
        }
    }
}
