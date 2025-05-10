using InventorySystemWebUI.Models;
using InventorySystemWebUI.ViewModel;
using Newtonsoft.Json;
using System.Text;

namespace InventorySystemWebUI.Service;

public interface IGeneralService
{
  Task<string> GetUserInfo(User user);
}

public class GeneralService : IGeneralService
{
    private readonly IConfiguration _config;
    private string apiUrl = "";
    public User User { get; set; } = new();

    public GeneralService(IConfiguration config)
    {
        _config = config;
        apiUrl = _config.GetValue<string>("BaseUrl")!;
    }

    public async Task<string> GetUserInfo(User user)
    {
        var result = "";
        using (HttpClient client = new HttpClient())
        {
            var endPoint = apiUrl + "/api/Authentication/token";
            StringContent body = new StringContent(JsonConvert.SerializeObject(user),
                Encoding.UTF8, "application/json");
            using (var Response = await client.PostAsync(endPoint, body))
            {
                if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    result = await Response.Content.ReadAsStringAsync();
                    return result;
                }
            }
        }
        return "";
    }
}
