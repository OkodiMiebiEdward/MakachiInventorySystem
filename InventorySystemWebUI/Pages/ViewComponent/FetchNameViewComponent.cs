using InventorySystemWebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace InventorySystemWebUI.Pages.ViewComponents
{
    public class FetchNameViewComponent:ViewComponent
    {
        private readonly IConfiguration _config;
        private readonly string apiUrl = "";

        public FetchNameViewComponent(IConfiguration config)
        {
            _config = config;
            apiUrl = _config.GetValue<string>("BaseUrl")!;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            string token = HttpContext.Session.GetString("AuthToken")!;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                              new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer",
                              $"{token}");
                    var endPoint = apiUrl + "/api/Identity/GetRoleFromSignedIn";

                    using (var Response = await client.GetAsync(endPoint))
                    {
                        if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var json = await Response.Content.ReadAsStringAsync();
                            var model = JsonConvert.DeserializeObject<SignedInDetail>(json);
                            return View("Fetch", model);
                        }
                        else
                        {
                            var json = await Response.Content.ReadAsStringAsync();
                            var model = JsonConvert.DeserializeObject<SignedInDetail>(json);
                            return View("Fetch", model);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return View("Default", new SignedInDetail());
            }
        }
    }
}
