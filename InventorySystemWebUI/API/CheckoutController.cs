using InventorySystemWebUI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using SkiaSharp;
using System.Text;

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

        [HttpPost("PrintReceipt")]
        public async Task<IActionResult> SaleCheckout([FromBody] Checkout checkout)
        {
            try
            {
                if (checkout is null)
                {
                    return BadRequest(new ResponseModel
                    {
                        Status = "Failed",
                        Description = "Provide valid data"
                    });
                }
                else
                {
                    if (checkout.SubData.Count > 0)
                    {
                        var (success, receiptDetail) = await GenerateReceipt(checkout);
                        if (success)
                        {
                            // Generate PDF
                            var pdfBytes = GenerateReceiptPdf(receiptDetail);
                            // Return PDF as file
                            return File(pdfBytes, "application/pdf", "Receipt.pdf");
                        }
                        else
                        {
                            return BadRequest(new ResponseModel
                            {
                                Status = "Failed",
                                Description = "Receipt generation failed"
                            });
                        }

                    }
                    else
                    {
                        return BadRequest(new ResponseModel
                        {
                            Status = "Failed",
                            Description = "No items provided to be checked out"
                        });
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<(bool, Checkout)> GenerateReceipt(Checkout checkout)
        {
            var result = new Checkout();
            string token = HttpContext.Session.GetString("AuthToken")!;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                              new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer", $"{token}");
                    var endPoint = apiUrl + "/api/Sales/Checkout";
                    StringContent body = new StringContent(JsonConvert.SerializeObject(checkout)
                        , Encoding.UTF8, "application/json");

                    using (var Response = await client.PostAsync(endPoint, body))
                    {
                        if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var json = await Response.Content.ReadAsStringAsync();
                            result = JsonConvert.DeserializeObject<Checkout>(json)!;
                            return (true, result);
                        }
                        else
                        {
                            var json = await Response.Content.ReadAsStringAsync();
                            result = JsonConvert.DeserializeObject<Checkout>(json)!;
                            return (false, result);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return (false, result);
            }
        }

        private byte[] GenerateReceiptPdf(Checkout checkout)
        {
            string storeName = _config.GetValue<string>("Supermarket:Name") ?? "Supermarket Name";
            string storeAddress = _config.GetValue<string>("Supermarket:Address") ?? "123 Main St, City";
            string storePhone = _config.GetValue<string>("Supermarket:Phone") ?? "Tel: 123-456-7890";

            // Set custom receipt size (e.g., 80mm x 200mm)
            const double mmToPoint = 2.83465;
            double width = 80 * mmToPoint;   // 80mm
            double height = 100 * mmToPoint; // 200mm

            using (var ms = new MemoryStream())
            {
                var document = new PdfDocument();
                var page = document.AddPage();
                page.Width = width;
                page.Height = height;

                var gfx = XGraphics.FromPdfPage(page);
                var fontTitle = new XFont("Verdana", 10, XFontStyle.Bold);
                var font = new XFont("Verdana", 8, XFontStyle.Regular);
                var fontSmall = new XFont("Verdana", 7, XFontStyle.Regular);

                double y = 10;
                // Supermarket Name (centered)
                gfx.DrawString(storeName, fontTitle, XBrushes.Black,
                    new XRect(0, y, page.Width, 12), XStringFormats.TopCenter);
                y += 12;

                // Address (centered)
                gfx.DrawString(storeAddress, font, XBrushes.Black,
                    new XRect(0, y, page.Width, 10), XStringFormats.TopCenter);
                y += 10;

                // Phone (centered)
                gfx.DrawString(storePhone, font, XBrushes.Black,
                    new XRect(0, y, page.Width, 10), XStringFormats.TopCenter);
                y += 12;

                // Date and Time
                string dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                gfx.DrawString($"Date: {dateTime}", fontSmall, XBrushes.Black,
                    new XRect(0, y, page.Width, 8), XStringFormats.TopCenter);
                y += 10;

                // Receipt title
                gfx.DrawString("RECEIPT", fontTitle, XBrushes.Black,
                    new XRect(0, y, page.Width, 12), XStringFormats.TopCenter);
                y += 14;

                // Table header
                gfx.DrawString("Qty  Product           Amount", fontSmall, XBrushes.Black,
                    new XRect(5, y, page.Width - 10, 8), XStringFormats.TopLeft);
                y += 10;

                decimal total = 0;
                foreach (var item in checkout.SubData)
                {
                    // Format: 2 X Drinks   1,300
                    string line = $"{item.Quantity} X {item.ProductName}".PadRight(18);
                    string amount = $"{item.FinalPrice:N0}";
                    gfx.DrawString(line, fontSmall, XBrushes.Black,
                        new XRect(5, y, 60, 8), XStringFormats.TopLeft);
                    gfx.DrawString(amount, fontSmall, XBrushes.Black,
                        new XRect(page.Width - 45, y, 40, 8), XStringFormats.TopRight);
                    y += 10;
                    total += item.FinalPrice;
                }

                y += 6;
                gfx.DrawLine(XPens.Black, 5, y, page.Width - 5, y);
                y += 4;

                // Total
                gfx.DrawString("TOTAL", fontTitle, XBrushes.Black,
                    new XRect(5, y, 60, 10), XStringFormats.TopLeft);
                gfx.DrawString($"{total:N2}", fontTitle, XBrushes.Black,
                    new XRect(page.Width - 45, y, 40, 10), XStringFormats.TopRight);
                y += 14;

                // Issuer at the bottom
                y = page.Height - 20;
                gfx.DrawString($"Issuer: {checkout.LoggedInUser}", fontSmall, XBrushes.Black,
                    new XRect(5, y, page.Width - 10, 8), XStringFormats.TopLeft);

                document.Save(ms, false);
                return ms.ToArray();
            }
        }

        //        var productStocks = _inventoryDbContext.Stocks
        //            .Include(s => s.Product)
        //            .Where(s => s.Product.BarCodeNumber == sales.Barcodenumber)
        //            .ToList();

        //        if (productStocks.Count > 0)
        //        {
        //            int totalQuantity = productStocks.Sum(s => s.QuantityInStock);
        //            if ((totalQuantity >= sales.Quantity) || (totalQuantity >= 0))
        //            {
        //                var stockToUpdate = productStocks
        //                    .OrderBy(s => s.CreatedAt)
        //                    .FirstOrDefault(s => s.QuantityInStock >= sales.Quantity);

        //                if (stockToUpdate != null)
        //                {
        //                    stockToUpdate.QuantityInStock += sales.Quantity;
        //                    await _inventoryDbContext.SaveChangesAsync();
        //                    return StatusCode(200, new ResponseModel 
        //                    { 
        //                      Status = "Success",
        //                      Description = "Save operation successful"
        //                    });
        //                }
        //                else
        //                {
        //                    return StatusCode(400, new ResponseModel
        //                    {
        //                        Status = "Failed",
        //                        Description = "No individual stock row has enough quantity"
        //                    });
        //                }
        //            }
        //            else
        //            {
        //                return StatusCode(400, new ResponseModel
        //                {
        //                    Status = "Failed",
        //                    Description = "No individual stock row has enough quantity"
        //                });
        //            }
        //        }
        //        else
        //            return StatusCode(400, new ResponseModel
        //            {
        //                Status = "Failed",
        //                Description = $"No product attached to this barcode {sales.Barcodenumber}"
        //            });
        //        #endregion
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(500, new ResponseModel
        //        {
        //            Status = "ServerError",
        //            Description = serverErrorMessage
        //        });
        //    }

    }
}
