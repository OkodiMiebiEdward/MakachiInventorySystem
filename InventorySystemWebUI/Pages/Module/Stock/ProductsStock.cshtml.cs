using InventorySystemWebUI.Models;
using InventorySystemWebUI.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Text;
using System.Xml.Linq;

namespace InventorySystemWebUI.Pages.Module.Stock
{
    public class ProductsStockModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly string apiUrl = "";

        public ResponseModel ResponseModel { get; set; } = new();

        [BindProperty]
        public StockVM Stock { get; set; } = new();

        [BindProperty]
        public List<CategoryVM> Categories { get; set; } = new();

        [BindProperty]
        public List<ProductVM> Products { get; set; } = new();

        public string CanDelete = "No";

        public ProductsStockModel(IConfiguration config)
        {
            _config = config;
            apiUrl = _config.GetValue<string>("BaseUrl")!;
        }

        public async Task<IActionResult> OnGet(int? id)
        {
            if (id is not null)
            {
                CanDelete = "Yes";
                Stock = await GetSingleStock(id);
                return Page();
            }

            Categories = await PopulateCategories();
            Products = await PopulateProducts();
            return Page();
        }

        private async Task<StockVM> GetSingleStock(int? id)
        {
            Categories = await PopulateCategories();
            string token = HttpContext.Session.GetString("AuthToken")!;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                              new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer", $"{token}");
                    var endPoint = apiUrl + $"/api/Stock/GetStock?id={id}";

                    using (var Response = await client.GetAsync(endPoint))
                    {
                        if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var json = await Response.Content.ReadAsStringAsync();
                            Stock = JsonConvert.DeserializeObject<StockVM>(json)!;
                            return Stock;
                        }
                        else
                        {
                            var json = await Response.Content.ReadAsStringAsync();
                            Stock = JsonConvert.DeserializeObject<StockVM>(json)!;
                            return Stock;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return new StockVM();
            }
        }

        private async Task<List<CategoryVM>> PopulateCategories()
        {
            string token = HttpContext.Session.GetString("AuthToken")!;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                              new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer", $"{token}");
                    var endPoint = apiUrl + "/api/Categories/GetCategories";

                    using (var Response = await client.GetAsync(endPoint))
                    {
                        if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var json = await Response.Content.ReadAsStringAsync();
                            Categories = JsonConvert.DeserializeObject<List<CategoryVM>>(json)!;
                        }
                        else
                        {
                            var json = await Response.Content.ReadAsStringAsync();
                            Categories = JsonConvert.DeserializeObject<List<CategoryVM>>(json)!;
                        }
                    }
                }
            }
            catch (Exception)
            {
                Categories = new List<CategoryVM>();
            }
            return Categories;
        }

        private async Task<List<ProductVM>> PopulateProducts()
        {
            string token = HttpContext.Session.GetString("AuthToken")!;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                              new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer", $"{token}");
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
                Products = new List<ProductVM>();
            }
            return Products;
        }

        public async Task<ActionResult> OnPostAsync()
        {
            string token = HttpContext.Session.GetString("AuthToken")!;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                              new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer", $"{token}");
                    var endPoint = apiUrl + "/api/Stock/CreateStock";
                    StringContent body = new StringContent(JsonConvert.SerializeObject(Stock)
                        , Encoding.UTF8, "application/json");

                    using (var Response = await client.PostAsync(endPoint, body))
                    {
                        if (Response.StatusCode == System.Net.HttpStatusCode.Created)
                        {
                            var json = await Response.Content.ReadAsStringAsync();
                            ResponseModel = JsonConvert.DeserializeObject<ResponseModel>(json)!;
                            return Page();
                        }
                        else
                        {
                            var json = await Response.Content.ReadAsStringAsync();
                            ResponseModel = JsonConvert.DeserializeObject<ResponseModel>(json)!;
                            return Page();
                        }
                    }
                }
            }
            catch (Exception)
            {
                ResponseModel.Status = "ServerError";
                ResponseModel.Description = "An unexpected error occurred";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDelete()
        {
            string token = HttpContext.Session.GetString("AuthToken")!;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                              new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer", $"{token}");
                    var endPoint = apiUrl + $"/api/Stock/DeleteStock?id={Stock?.Id}";

                    using (var Response = await client.DeleteAsync(endPoint))
                    {
                        if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var json = await Response.Content.ReadAsStringAsync();
                            ResponseModel = JsonConvert.DeserializeObject<ResponseModel>(json)!;
                            return Page();
                        }
                        else
                        {
                            var json = await Response.Content.ReadAsStringAsync();
                            ResponseModel = JsonConvert.DeserializeObject<ResponseModel>(json)!;
                            return Page();
                        }
                    }
                }
            }
            catch (Exception)
            {
                ResponseModel.Status = "ServerError";
                ResponseModel.Description = "An unexpected error occurred";
                return Page();
            }
        }
    }
}
