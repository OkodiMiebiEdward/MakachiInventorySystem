using InventorySystemWebUI.Models;
using InventorySystemWebUI.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace InventorySystemWebUI.Pages.Dashboard
{
    public class UsersModel : PageModel
    {
        public ResponseModel ResponseModel { get; set; } = new();
        private readonly IConfiguration _config;
        private readonly string apiUrl;

        public List<User> Users { get; set; } = new();
        public int TotalUsers { get; set; }

        public UsersModel(IConfiguration config)
        {
            _config = config;
            apiUrl = _config.GetValue<string>("BaseUrl")!;
        }

        public async Task<IActionResult> OnGet()
        {
            await GetAllUsers();
            return Page();
        }

        private async Task GetAllUsers()
        {
            string token = HttpContext.Session.GetString("AuthToken")!;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                              new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer",
                              $"{token}");
                    var endPoint = apiUrl + "/api/Identity/GetAllUsers";

                    using (var Response = await client.GetAsync(endPoint))
                    {
                        if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var json = await Response.Content.ReadAsStringAsync();
                            Users = JsonConvert.DeserializeObject<List<User>>(json)!;
                            TotalUsers = Users.Count;
                        }
                        else
                        {
                            var json = await Response.Content.ReadAsStringAsync();
                            Users = JsonConvert.DeserializeObject<List<User>>(json)!;
                            TotalUsers = Users.Count;
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

