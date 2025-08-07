using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystemWebUI.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly string apiUrl = "";

        public CheckoutController(IConfiguration config)
        {
            _config = config;
            apiUrl = _config.GetValue<string>("BaseUrl")!;
        }

        //[HttpPost("ProductsCheckout")]
        //public Task<IActionResult> ProductsCheckout([FromBody])
        //{
        //    try
        //    {

        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
    }
}
