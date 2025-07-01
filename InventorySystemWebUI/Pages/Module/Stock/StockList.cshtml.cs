using InventorySystemWebUI.Models;
using InventorySystemWebUI.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace InventorySystemWebUI.Pages.Module.Stock
{
    public class StockListModel : PageModel
    {
        public ResponseModel ResponseModel { get; set; } = new();
        private readonly IConfiguration _config;
        private readonly string apiUrl = "";

        [BindProperty]
        public List<StockVM> Stocks { get; set; } = new();

        public StockListModel(IConfiguration config)
        {
            _config = config;
            apiUrl = _config.GetValue<string>("BaseUrl")!;
        }

        public async Task<IActionResult> OnGet()
        {
            await GetStocks();
            return Page();
        }

        private async Task GetStocks()
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
                            Stocks = JsonConvert.DeserializeObject<List<StockVM>>(json)!;
                        }
                        else
                        {
                            var json = await Response.Content.ReadAsStringAsync();
                            Stocks = JsonConvert.DeserializeObject<List<StockVM>>(json)!;
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
