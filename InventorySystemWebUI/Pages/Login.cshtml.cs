using InventorySystemWebUI.Models;
using InventorySystemWebUI.Service;
using InventorySystemWebUI.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Text;

namespace InventorySystemWebUI.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly IGeneralService _generalService;
        private string apiUrl = "";
        public ResponseModel ResponseModel { get; set; } = new();

        public LoginModel(IConfiguration config, IGeneralService generalService)
        {
            _config = config;
            _generalService = generalService;
            apiUrl = _config.GetValue<string>("BaseUrl")!;
        }

        [BindProperty]
        public User User { get; set; } = new User();

        public ActionResult OnGet()
        {
            return Page();
        }

        public async Task<ActionResult> OnPost()
        {
            var token = await _generalService.GetUserInfo(User);
            if(string.IsNullOrWhiteSpace(token))
                return Page();

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                          new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer", $"{token}");
                var endPoint = apiUrl + "/api/Identity/Login";
                StringContent body = new StringContent(JsonConvert.SerializeObject(User)
                    , Encoding.UTF8, "application/json");

                using (var Response = await client.PostAsync(endPoint, body))
                {
                    if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var json = await Response.Content.ReadAsStringAsync();
                        ResponseModel = JsonConvert.DeserializeObject<ResponseModel>(json)!;
                        if (ResponseModel.Status == "Success")
                            return Page();
                        else
                        {
                            return Page();
                        }
                    }
                    else if (Response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        var json = await Response.Content.ReadAsStringAsync();
                        ResponseModel = JsonConvert.DeserializeObject<ResponseModel>(json)!;
                        if (ResponseModel.Status == "Failed")
                            return Page();
                    }
                }
            }
            return Page();
        }
    }
}
