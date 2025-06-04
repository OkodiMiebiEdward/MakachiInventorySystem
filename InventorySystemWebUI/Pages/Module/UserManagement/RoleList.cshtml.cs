using InventorySystemWebUI.Models;
using InventorySystemWebUI.Service;
using InventorySystemWebUI.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Data;
using System.Text;

namespace InventorySystemWebUI.Pages.Module.UserManagement
{
    public class RoleListModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly IGeneralService _generalService;
        private readonly string apiUrl = "";
        public ResponseModel ResponseModel { get; set; } = new ResponseModel();

        [BindProperty]
        public List<RoleVM> Roles { get; set; }

        public RoleListModel(IConfiguration config,
            IGeneralService generalService)
        {
            _config = config;
            _generalService = generalService;
            apiUrl = _config.GetValue<string>("BaseUrl")!;
        }

        public async Task<ActionResult> OnGet()
        {
            return Page();
        }

        private async Task GetRoles()
        {
            string token = HttpContext.Session.GetString("AuthToken")!;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                              new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer",
                              $"{token}");
                    var endPoint = apiUrl + "/api/Identity/GetRoles";

                    using (var Response = await client.GetAsync(endPoint))
                    {
                        if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var json = await Response.Content.ReadAsStringAsync();
                            ResponseModel = JsonConvert.DeserializeObject<ResponseModel>(json)!;
                        }
                        else if (Response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                        {
                            var json = await Response.Content.ReadAsStringAsync();
                            ResponseModel = JsonConvert.DeserializeObject<ResponseModel>(json)!;
                        }
                        else
                        {
                            var json = await Response.Content.ReadAsStringAsync();
                            ResponseModel = JsonConvert.DeserializeObject<ResponseModel>(json)!;
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
