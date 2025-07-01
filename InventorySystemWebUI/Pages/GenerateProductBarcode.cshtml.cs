using BarcodeStandard;
using InventorySystemWebUI.Models;
using InventorySystemWebUI.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;

namespace InventorySystemWebUI.Pages
{
    public class GenerateProductBarcodeModel : PageModel
    {
        public ResponseModel ResponseModel { get; set; } = new();
        private readonly IConfiguration _config;
        private readonly string apiUrl;

        [BindProperty]
        public List<ProductVM> Products { get; set; } = new();

        public GenerateProductBarcodeModel(IConfiguration config)
        {
            _config = config;
            apiUrl = _config.GetValue<string>("BaseUrl")!;
        }

        public async Task<IActionResult> OnGetAsync()
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

        public async Task<IActionResult> OnGetGenerateBarcodeAsync(string code)
        {
            string token = HttpContext.Session.GetString("AuthToken")!;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var endPoint = apiUrl + "/api/BarCodeGenerator/GenerateBarcode?barCodeNumber=" + Uri.EscapeDataString(code);

                    using (var response = await client.GetAsync(endPoint))
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var imageBytes = await response.Content.ReadAsByteArrayAsync();
                            var base64 = Convert.ToBase64String(imageBytes);
                            return new JsonResult(new { success = true, base64 });
                        }
                        else
                        {
                            var error = await response.Content.ReadAsStringAsync();
                            return new JsonResult(new { success = false, error });
                        }
                    }
                }
            }
            catch (Exception)
            {
                return new JsonResult(new { success = false, error = "An unexpected error occurred" });
            }
        }
    }
}
