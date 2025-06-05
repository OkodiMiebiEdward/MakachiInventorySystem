using InventorySystemWebUI.Models;
using InventorySystemWebUI.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Text;

namespace InventorySystemWebUI.Pages.Module.UserManagement
{
    public class CreateUserModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly string apiUrl = "";
        public ResponseModel ResponseModel { get; set; } = new ResponseModel();

        [BindProperty]
        public List<User> Users { get; set; } = new();

        [BindProperty]
        public List<RoleVM> Roles { get; set; } = new();

        [BindProperty]
        public AssignRoleVM AssignRole { get; set; } = new();

        public CreateUserModel(IConfiguration config)
        {
            _config = config;
            apiUrl = _config.GetValue<string>("BaseUrl")!;
        }

        public async Task<ActionResult> OnGet()
        {
            Users = await PopulateUsers();
            Roles = await PopulateRoles();
            return Page();
        }

        private async Task<List<User>> PopulateUsers()
        {
            string token = HttpContext.Session.GetString("AuthToken")!;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                              new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer", $"{token}");
                    var endPoint = apiUrl + "/api/Identity/GetAllUsers";

                    using (var Response = await client.GetAsync(endPoint))
                    {
                        if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var json = await Response.Content.ReadAsStringAsync();
                            Users = JsonConvert.DeserializeObject<List<User>>(json)!;
                        }
                        else
                        {
                            var json = await Response.Content.ReadAsStringAsync();
                            Users = JsonConvert.DeserializeObject<List<User>>(json)!;
                        }
                    }
                }
            }
            catch (Exception)
            {
                Users = new List<User>();
            }
            return Users;
        }

        private async Task<List<RoleVM>> PopulateRoles()
        {
            string token = HttpContext.Session.GetString("AuthToken")!;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                              new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer", $"{token}");
                    var endPoint = apiUrl + "/api/Identity/GetRoles";

                    using (var Response = await client.GetAsync(endPoint))
                    {
                        if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var json = await Response.Content.ReadAsStringAsync();
                            Roles = JsonConvert.DeserializeObject<List<RoleVM>>(json)!;
                        }
                        else
                        {
                            var json = await Response.Content.ReadAsStringAsync();
                            Roles = JsonConvert.DeserializeObject<List<RoleVM>>(json)!;
                        }
                    }
                }
            }
            catch (Exception)
            {
                Roles = new List<RoleVM>();
            }
            return Roles;
        }

        public async Task<IActionResult> OnPost()
        {
            string token = HttpContext.Session.GetString("AuthToken")!;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                              new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer", $"{token}");
                    var endPoint = apiUrl + "/api/Identity/AssignRolesToUsers";
                    StringContent body = new StringContent(JsonConvert.SerializeObject(AssignRole)
                        , Encoding.UTF8, "application/json");

                    using (var Response = await client.PostAsync(endPoint, body))
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
