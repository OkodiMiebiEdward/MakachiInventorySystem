using InventorySystemWebUI.Models;
using InventorySystemWebUI.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Data;
using System.Text;

namespace InventorySystemWebUI.Pages.Module.Product
{
    public class CreateCategoryModel : PageModel
    {
        private readonly string apiUrl = "";
        private readonly IConfiguration _config;

        public ResponseModel ResponseModel { get; set; } = new();

        [BindProperty]
        public CategoryVM Category { get; set; } = new();

        public string CanDelete = "No";

        public CreateCategoryModel(IConfiguration config)
        {
            _config = config;
            apiUrl = _config.GetValue<string>("BaseUrl")!;
        }

        public async Task<IActionResult> OnGet(int? id)
        {
            if (id is not null)
            {
                CanDelete = "Yes";
                Category = await GetSingleCategory(id);
            }
            return Page();
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
                    var endPoint = apiUrl + "/api/Categories/CreateCategory";
                    StringContent body = new StringContent(JsonConvert.SerializeObject(Category)
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

        private async Task<CategoryVM> GetSingleCategory(int? id)
        {
            string token = HttpContext.Session.GetString("AuthToken")!;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                              new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer", $"{token}");
                    var endPoint = apiUrl + $"/api/Categories/GetCategory?id={id}";

                    using (var Response = await client.GetAsync(endPoint))
                    {
                        if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var json = await Response.Content.ReadAsStringAsync();
                            Category = JsonConvert.DeserializeObject<CategoryVM>(json)!;
                            return Category;
                        }
                        else
                        {
                            var json = await Response.Content.ReadAsStringAsync();
                            Category = JsonConvert.DeserializeObject<CategoryVM>(json)!;
                            return Category;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return new CategoryVM();
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
                    var endPoint = apiUrl + $"/api/Categories/DeleteCategory?id={Category?.Id}";

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
