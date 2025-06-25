using InventorySystemWebUI.Models;
using InventorySystemWebUI.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace InventorySystemWebUI.Pages.Module.Product
{
    public class ProductListModel : PageModel
    {

        public ResponseModel ResponseModel { get; set; } = new();
        private readonly IConfiguration _config;
        private readonly string apiUrl;

        [BindProperty]
        public List<ProductVM> Products { get; set; } = new();

        public ProductListModel(IConfiguration config)
        {
            _config = config;
            apiUrl = _config.GetValue<string>("BaseUrl")!;
        }

        public async Task<IActionResult> OnGet()
        {
            await GetProducts();
            return Page();
        }

        private async Task GetProducts()
        {
            string token = HttpContext.Session.GetString("AuthToken")!;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                              new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer",
                              $"{token}");
                    var endPoint = apiUrl + "/api/Products/GetProducts";

                    using (var Response = await client.GetAsync(endPoint))
                    {
                        if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var json = await Response.Content.ReadAsStringAsync();
                            Products = JsonConvert.DeserializeObject<List<ProductVM>>(json)!;
                        }
                        else
                        {
                            var json = await Response.Content.ReadAsStringAsync();
                            Products = JsonConvert.DeserializeObject<List<ProductVM>>(json)!;
                        }
                    }
                }
            }
            catch (Exception)
            {
                ResponseModel.Status = "ServerError";
                ResponseModel.Description = "An unexpected error occurred";
            }
        }
    }
}
