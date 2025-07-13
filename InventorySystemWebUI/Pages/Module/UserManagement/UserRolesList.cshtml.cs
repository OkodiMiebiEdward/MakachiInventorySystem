using InventorySystemWebUI.Models;
using InventorySystemWebUI.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Data;

namespace InventorySystemWebUI.Pages.Module.UserManagement
{
    public class UserRolesListModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly string apiUrl = "";

        [BindProperty]
        public List<AssignRoleVM> UserRoleRecords { get; set; } = new();

        [BindProperty]
        public List<UserWithRolesVM> UsersWithRoles { get; set; }

        public UserRolesListModel(IConfiguration config)
        {
            _config = config;
            apiUrl = _config.GetValue<string>("BaseUrl")!;
        }

        public async Task<IActionResult> OnGet()
        {
            UserRoleRecords = await GetUsersRoles();
            UsersWithRoles = UserRoleRecords
            .GroupBy(x => x.UserName)
            .Select(g => new UserWithRolesVM
            {
                UserName = g.Key,
                Roles = g.Select(x => x.Role).ToList()
            })
            .ToList();
            return Page();
        }

        private async Task<List<AssignRoleVM>> GetUsersRoles()
        {
            string token = HttpContext.Session.GetString("AuthToken")!;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                              new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer",
                              $"{token}");
                    var endPoint = apiUrl + "/api/Identity/GetUsersAndRoles";

                    using (var Response = await client.GetAsync(endPoint))
                    {
                        if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var json = await Response.Content.ReadAsStringAsync();
                            UserRoleRecords = JsonConvert.DeserializeObject<List<AssignRoleVM>>(json)!;
                            return UserRoleRecords;
                        }
                        else
                        {
                            var json = await Response.Content.ReadAsStringAsync();
                            UserRoleRecords = JsonConvert.DeserializeObject<List<AssignRoleVM>>(json)!;
                            return UserRoleRecords;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return new List<AssignRoleVM>();
            }
        }
    }
}
