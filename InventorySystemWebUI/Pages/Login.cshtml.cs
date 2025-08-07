using InventorySystemWebUI.Models;
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
        private string apiUrl = "";

        [BindProperty]
        public User User { get; set; } = new User();

        [BindProperty]
        public ResponseModel ResponseModel { get; set; } = new();

        public LoginModel(IConfiguration config)
        {
            _config = config;
            apiUrl = _config.GetValue<string>("BaseUrl")!;
        }

        public ActionResult OnGet()
        {
            return Page();
        }

        public async Task<ActionResult> OnPost()
        {
            try
            {
                if (!ModelState.IsValid)
                    return Page();

                using (HttpClient client = new HttpClient())
                {
                    var endPoint = apiUrl + "/api/Identity/CreateUser";
                    StringContent body = new StringContent(JsonConvert.SerializeObject(User)
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
                            ResponseModel.Status = "Failed";
                            ResponseModel.Description = "User creation failed";
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
