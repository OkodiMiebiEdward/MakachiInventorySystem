using InventorySystemWebUI.Models;
using InventorySystemWebUI.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.Json;

namespace InventorySystemWebUI.Pages.Module.Sales
{
    public class CreateSaleModel : PageModel
    {
        private readonly IConfiguration _config;

        public ResponseModel ResponseModel { get; set; } = new();
        private readonly string apiUrl = "";

        [BindProperty]
        public StockVM Product { get; set; } = new();
        public SalesVM Sale { get; set; } = new();

        public CreateSaleModel(IConfiguration config)
        {
            _config = config;
            apiUrl = _config.GetValue<string>("BaseUrl")!;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnGetFetchProductFromBarcodenumber(string barcodenumber)
        {
            string div = "";
            string json = "";
            try
            {
                var product = await GetProduct(barcodenumber);
                if (product is not null)
                {
                   div = $@"
                    <div class='modal-dialog' role='document'>
                        <div class='modal-content' style='width:700px'>
                            <div class='modal-header'>
                                <h5 class='modal-title' id='exampleModalLabel1'>Product: {product.ProductName}</h5>
                                <button type='button' class='btn btn-danger' data-bs-dismiss='modal' onclick='getDataFromModal(0)' aria-label='Close'>Close</button>
                            </div>
                            <div class='modal-body'>
                                <p><strong>Stock Number:</strong> {product.StockNumber}</p>
                                <p><strong>BarcodeNumber:</strong> {product.BarCodeNumber}</p>
                            </div>
                        </div>
                   </div>";

                    #region TableSection
                    //    //+ $" <p><strong>Barcode:</strong> {product.BarCodeNumber}</p>"
                    //    + " <div class='col-md-12'>"
                    //    + " <div class='table-responsive'>"
                    //    + " <table id='dtabModal' class='table table-hover'>"
                    //    + " <thead><tr>"
                    //    + " <th style='text-align:center;padding:10px; border:1px solid lightgrey;'>S/N</th>"
                    //    + " <th style='text-align:center;padding:10px; border:1px solid lightgrey;'>Size</th>"
                    //    + " <th style='text-align:center;padding:10px; border:1px solid lightgrey;'>Color</th>"
                    //    + " </tr></thead>"
                    //    + " <tbody>";

                    //if (product.Variants != null && product.Variants.Count > 0)
                    //{
                    //    int sn = 1;
                    //    foreach (var variant in product.Variants)
                    //    {
                    //        div += $"<tr>"
                    //            + $"<td style='text-align:center;padding:10px; border:1px solid lightgrey;'>{sn++}</td>"
                    //            + $"<td style='text-align:center;padding:10px; border:1px solid lightgrey;'>{variant.Size}</td>"
                    //            + $"<td style='text-align:center;padding:10px; border:1px solid lightgrey;'>{variant.Color}</td>"
                    //            + $"</tr>";
                    //    }
                    //}
                    //else
                    //{
                    //    div += "<tr><td colspan='4' style='text-align:center;'>No variants available.</td></tr>";
                    //}

                    //div += "</tbody></table></div></div></div></div></div>";
                    #endregion
                }
                json = System.Text.Json.JsonSerializer.Serialize(new { Html = div, Discount = product?.Discount, Price = product?.SellingUnitPrice},
                    new JsonSerializerOptions() { WriteIndented = true });
            }
            catch (Exception)
            {
                return StatusCode(400);
            }
            return new JsonResult(json);
        }

        private async Task<StockVM> GetProduct(string codenumber)
        {
            StockVM product = new();
            string token = HttpContext.Session.GetString("AuthToken")!;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                              new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer", $"{token}");
                    var endPoint = apiUrl + $"/api/Products/GetProductByBarCodeNumber?barcodenumber={codenumber}";

                    using (var Response = await client.GetAsync(endPoint))
                    {
                        if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var json = await Response.Content.ReadAsStringAsync();
                            product = JsonConvert.DeserializeObject<StockVM>(json)!;
                            Product = product;
                            return product;
                        }
                        else
                        {
                            var json = await Response.Content.ReadAsStringAsync();
                            product = JsonConvert.DeserializeObject<StockVM>(json)!;
                            return Product;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return new StockVM();
            }
        }

        public async Task<IActionResult> OnGetInitiateSale(decimal discount, decimal priceSold,
            int quantity, string barcode)
        {
            SalesVM sale = new SalesVM
            {
                Discount = discount,
                PriceSold = priceSold,
                Quantity = quantity,
                Barcodenumber = barcode
            };

            #region FinalSellingPriceCalculation
            var discountedPrice = (discount / 100) * priceSold;
            var finalPrice = (priceSold - discountedPrice) * quantity;
            #endregion


            string val = "";
            string token = HttpContext.Session.GetString("AuthToken")!;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                              new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer", $"{token}");
                    var endPoint = apiUrl + "/api/Sales/CreateSale";
                    StringContent body = new StringContent(JsonConvert.SerializeObject(sale)
                        , Encoding.UTF8, "application/json");

                    using (var Response = await client.PostAsync(endPoint, body))
                    {
                        if (Response.StatusCode == System.Net.HttpStatusCode.Created)
                        {
                            var json = await Response.Content.ReadAsStringAsync();
                            Sale = JsonConvert.DeserializeObject<SalesVM>(json)!;

                            int sn = 0;

                            #region RenderSales
                            string row =
                            $"<tr data-sale-id='{Sale.Id}'>" +
                            $"<td class='sn'></td>" + // S/N will be set by JS
                            $"<td>{Sale.Barcodenumber}</td>" +
                            $"<td>{Sale.Quantity}</td>" +
                            $"<td>{Sale.PriceSold.ToString("N2")}</td>" +
                            $"<td>{Sale.Discount}</td>" +
                            $"<td>{finalPrice.ToString("N2")}</td>" +
                            $"<td><button type='button' class='btn btn-danger btn-sm remove-sale-row'>Remove</button></td>" +
                            $"</tr>";
                            #endregion

                            val = System.Text.Json.JsonSerializer.Serialize(row
                                , new JsonSerializerOptions() { WriteIndented = true });
                            return new JsonResult(val);
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

        public async Task<IActionResult> OnGetRemoveSale(decimal discount, decimal priceSold,
            int quantity, string barcode)
        {
            SalesVM sale = new SalesVM
            {
                Discount = discount,
                PriceSold = priceSold,
                Quantity = quantity,
                Barcodenumber = barcode
            };

            try
            {
                string val = "";
                string token = HttpContext.Session.GetString("AuthToken")!;

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                              new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer", $"{token}");
                    var endPoint = apiUrl + "/api/Sales/RemoveSale";
                    StringContent body = new StringContent(JsonConvert.SerializeObject(sale)
                        , Encoding.UTF8, "application/json");

                    using (var Response = await client.PostAsync(endPoint, body))
                    {
                        if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var json = await Response.Content.ReadAsStringAsync();
                            ResponseModel = JsonConvert.DeserializeObject<ResponseModel>(json)!;
                            val = System.Text.Json.JsonSerializer.Serialize(new { Result = ResponseModel }
                                , new JsonSerializerOptions() { WriteIndented = true });
                            return new JsonResult(val);
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
