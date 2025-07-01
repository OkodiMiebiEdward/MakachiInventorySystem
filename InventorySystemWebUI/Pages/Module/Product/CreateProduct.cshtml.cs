using InventorySystemWebUI.Models;
using InventorySystemWebUI.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Drawing;
using System.Text;
using System.Xml.Linq;

namespace InventorySystemWebUI.Pages.Module.Product
{

    public class CreateProductModel : PageModel
    {
        private readonly string apiUrl = "";
        private readonly IConfiguration _config;

        public ResponseModel ResponseModel { get; set; } = new();

        [BindProperty]
        public ProductVM Product { get; set; } = new();

        [BindProperty]
        public List<CategoryVM> Categories { get; set; } = new();

        public string CanDelete = "No";

        public Random random = new();

        public CreateProductModel(IConfiguration config)
        {
            _config = config;
            apiUrl = _config.GetValue<string>("BaseUrl")!;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id is not null)
            {
                CanDelete = "Yes";
                Product = await GetSingleProduct(id);
                return Page();
            }

            PopulateWithDummyData();
            Categories = await PopulateCategories();
            return Page();
        }

        private async Task<ProductVM> GetSingleProduct(int? id)
        {
            Categories = await PopulateCategories();
            string token = HttpContext.Session.GetString("AuthToken")!;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                              new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer", $"{token}");
                    var endPoint = apiUrl + $"/api/Products/GetProduct?id= {id}";

                    using (var Response = await client.GetAsync(endPoint))
                    {
                        if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var json = await Response.Content.ReadAsStringAsync();
                            Product = JsonConvert.DeserializeObject<ProductVM>(json)!;
                            return Product;
                        }
                        else
                        {
                            var json = await Response.Content.ReadAsStringAsync();
                            Product = JsonConvert.DeserializeObject<ProductVM>(json)!;
                            return Product;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return new ProductVM();
            }
        }

        private void PopulateWithDummyData()
        {
            List<VariantsVM> dummyVariants = [];
            dummyVariants.AddRange(new VariantsVM
            {
                Price = 0.00m,
                Size = "",
                Color = "",
            }
            );
            Product.Variants.AddRange(dummyVariants);
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

        public async Task<ActionResult> OnPostAsync()
        {
            string token = HttpContext.Session.GetString("AuthToken")!;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                              new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer", $"{token}");
                    var endPoint = apiUrl + "/api/Products/CreateProduct";
                    StringContent body = new StringContent(JsonConvert.SerializeObject(Product)
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
                    var endPoint = apiUrl + $"/api/Products/DeleteProduct?id={Product?.Id}";

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
