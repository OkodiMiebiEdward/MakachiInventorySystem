using InventorySystemWebUI.Models;
using InventorySystemWebUI.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace InventorySystemWebUI.Pages.Module.Analytics
{
    public class ViewInventoryModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly string apiUrl;

        public ViewInventoryModel(IConfiguration config)
        {
            _config = config;
            apiUrl = _config.GetValue<string>("BaseUrl")!;
        }
        
        public List<StockVM> Inventory { get; set; } = new();
        public async Task OnGetAsync()
        {
            string token = HttpContext.Session.GetString("AuthToken")!;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                              new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer",
                              $"{token}");
                    var endPoint = apiUrl + "/api/Stock/GetStocks";

                    using (var Response = await client.GetAsync(endPoint))
                    {
                        if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var json = await Response.Content.ReadAsStringAsync();
                            Inventory = JsonConvert.DeserializeObject<List<StockVM>>(json)!;
                        }
                        else
                        {
                            var json = await Response.Content.ReadAsStringAsync();
                            Inventory = JsonConvert.DeserializeObject<List<StockVM>>(json)!;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
