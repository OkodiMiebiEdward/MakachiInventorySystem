using InventorySystemWebUI.Models;
using InventorySystemWebUI.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystemWebUI.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _config;
        private string apiUrl = "";

        [BindProperty]
        public User User { get; set; } = new User();

        [BindProperty]
        public ResponseModel ResponseModel { get; set; } = new();   

        public IndexModel(IConfiguration config)
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
                        if (ResponseModel.Status == "Success")
                        {
                            return Page();
                        }
                    }
                    else
                        return Page();
                }
             }
            return Page();
        }
    }
}
