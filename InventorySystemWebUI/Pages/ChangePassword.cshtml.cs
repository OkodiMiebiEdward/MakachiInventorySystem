using InventorySystemWebUI.Models;
using InventorySystemWebUI.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Data;
using System.Text;

namespace InventorySystemWebUI.Pages
{
    public class ChangePasswordModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly string apiUrl = "";
        public ResponseModel ResponseModel { get; set; } = new ResponseModel();

        [BindProperty]
        public PasswordVM PasswordDetail { get; set; } = new();

        public ChangePasswordModel(IConfiguration config)
        {
            _config = config;
            apiUrl = _config.GetValue<string>("BaseUrl")!;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async  Task<IActionResult> OnPost()
        {
            string token = HttpContext.Session.GetString("AuthToken")!;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                              new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer", $"{token}");
                    var endPoint = apiUrl + "/api/Identity/ChangePassword";
                    StringContent body = new StringContent(JsonConvert.SerializeObject(PasswordDetail)
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
