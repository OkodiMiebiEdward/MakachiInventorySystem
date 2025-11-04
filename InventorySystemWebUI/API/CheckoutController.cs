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

                if (checkout.SubData == null || checkout.SubData.Count == 0)
                {
                    return BadRequest(new ResponseModel
                    {
                        Status = "Failed",
                        Description = "No items provided to be checked out"
                    });
                }

                var (success, receiptDetail) = await GenerateReceipt(checkout);
                if (!success)
                {
                    return BadRequest(new ResponseModel
                    {
                        Status = "Failed",
                        Description = "Receipt generation failed"
                    });
                }

                try
                {
                    // Generate PDF
                    var pdfBytes = GenerateReceiptPdf(receiptDetail ?? checkout);
                    // Send PDF with inline disposition so browsers can open it
                    Response.Headers["Content-Disposition"] = "inline; filename=Receipt.pdf";
                    return File(pdfBytes, "application/pdf");
                }
                catch (Exception ex)
                {
                    // Return a clear server error to the client (instead of rethrow)
                    return StatusCode(500, new ResponseModel
                    {
                        Status = "ServerError",
                        Description = $"PDF generation error: {ex.Message}"
                    });
                }
            }
            catch (Exception ex)
            {
                // Do not rethrow — return a 500 with info to help debugging on client
                return StatusCode(500, new ResponseModel
                {
                    Status = "ServerError",
                    Description = $"Unexpected server error: {ex.Message}"
                });
            }
        }

        private async Task<(bool, Checkout)> GenerateReceipt(Checkout checkout)
        {
            var result = new Checkout();
            string? token = HttpContext.Session.GetString("AuthToken");
            if (string.IsNullOrWhiteSpace(token))
            {
                // No token in session -> can't call backend
                return (false, result);
            }

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var endPoint = apiUrl + "/api/Sales/Checkout";
                    StringContent body = new StringContent(JsonConvert.SerializeObject(checkout)
                        , Encoding.UTF8, "application/json");

                    using (var Response = await client.PostAsync(endPoint, body))
                    {
                        var json = await Response.Content.ReadAsStringAsync();
                        if (Response.IsSuccessStatusCode)
                        {
                            result = JsonConvert.DeserializeObject<Checkout>(json)!;
                            return (true, result);
                        }
                        else
                        {
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

        //private byte[] GenerateReceiptPdf(Checkout checkout)
        //{
        //    try
        //    {
        //        string storeName = _config.GetValue<string>("Supermarket:Name") ?? "Supermarket Name";
        //        string storeAddress = _config.GetValue<string>("Supermarket:Address") ?? "123 Main St, City";
        //        string storePhone = _config.GetValue<string>("Supermarket:Phone") ?? "Tel: 123-456-7890";

        //        // Set custom receipt size (e.g., 80mm x 100mm)
        //        const double mmToPoint = 2.83465;
        //        double width = 80 * mmToPoint;   // 80mm
        //        double height = 100 * mmToPoint; // 100mm

        //        using (var ms = new MemoryStream())
        //        {
        //            var document = new PdfDocument();
        //            var page = document.AddPage();
        //            page.Width = width;
        //            page.Height = height;

        //            var gfx = XGraphics.FromPdfPage(page);
        //            var fontTitle = new XFont("Verdana", 10, XFontStyle.Bold);
        //            var font = new XFont("Verdana", 8, XFontStyle.Regular);
        //            var fontSmall = new XFont("Verdana", 7, XFontStyle.Regular);

        //            double y = 10;
        //            // Supermarket Name (centered)
        //            gfx.DrawString(storeName, fontTitle, XBrushes.Black,
        //                new XRect(0, y, page.Width, 12), XStringFormats.TopCenter);
        //            y += 12;

        //            // Address (centered)
        //            gfx.DrawString(storeAddress, font, XBrushes.Black,
        //                new XRect(0, y, page.Width, 10), XStringFormats.TopCenter);
        //            y += 10;

        //            // Phone (centered)
        //            gfx.DrawString(storePhone, font, XBrushes.Black,
        //                new XRect(0, y, page.Width, 10), XStringFormats.TopCenter);
        //            y += 12;

        //            // Date and Time
        //            string dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //            gfx.DrawString($"Date: {dateTime}", fontSmall, XBrushes.Black,
        //                new XRect(0, y, page.Width, 8), XStringFormats.TopCenter);
        //            y += 10;

        //            // Receipt title
        //            gfx.DrawString("RECEIPT", fontTitle, XBrushes.Black,
        //                new XRect(0, y, page.Width, 12), XStringFormats.TopCenter);
        //            y += 14;

        //            // Table header
        //            gfx.DrawString("Qty  Product           Amount", fontSmall, XBrushes.Black,
        //                new XRect(5, y, page.Width - 10, 8), XStringFormats.TopLeft);
        //            y += 10;

        //            decimal total = 0;
        //            if (checkout.SubData != null)
        //            {
        //                foreach (var item in checkout.SubData)
        //                {
        //                    // Format: 2 X Drinks   1,300
        //                    string line = $"{item.Quantity} X {item.ProductName}".PadRight(18);
        //                    string amount = $"{item.FinalPrice:N0}";
        //                    gfx.DrawString(line, fontSmall, XBrushes.Black,
        //                        new XRect(5, y, 60, 8), XStringFormats.TopLeft);
        //                    gfx.DrawString(amount, fontSmall, XBrushes.Black,
        //                        new XRect(page.Width - 45, y, 40, 8), XStringFormats.TopRight);
        //                    y += 10;
        //                    total += item.FinalPrice;
        //                }
        //            }

        //            y += 6;
        //            gfx.DrawLine(XPens.Black, 5, y, page.Width - 5, y);
        //            y += 4;

        //            // Total
        //            gfx.DrawString("TOTAL", fontTitle, XBrushes.Black,
        //                new XRect(5, y, 60, 10), XStringFormats.TopLeft);
        //            gfx.DrawString($"{total:N2}", fontTitle, XBrushes.Black,
        //                new XRect(page.Width - 45, y, 40, 10), XStringFormats.TopRight);
        //            y += 14;

        //            // Issuer at the bottom
        //            y = page.Height - 20;
        //            gfx.DrawString($"Issuer: {checkout.LoggedInUser}", fontSmall, XBrushes.Black,
        //                new XRect(5, y, page.Width - 10, 8), XStringFormats.TopLeft);

        //            document.Save(ms, false);
        //            return ms.ToArray();
        //        }
        //    }
        //    catch
        //    {
        //        // Bubble up to caller who will return a proper 500
        //        throw;
        //    }
        //}
        //private byte[] GenerateReceiptPdf(Checkout checkout)
        //{
        //    try
        //    {
        //        string storeName = _config.GetValue<string>("Supermarket:Name") ?? "Supermarket Name";
        //        string storeAddress = _config.GetValue<string>("Supermarket:Address") ?? "123 Main St, City";
        //        string storePhone = _config.GetValue<string>("Supermarket:Phone") ?? "Tel: 123-456-7890";
        //        // New: opening days and hours (will fall back to sensible defaults)
        //        string storeOpenDays = _config.GetValue<string>("Supermarket:OpeningDays") ?? "Mon-Sat";
        //        string storeHours = _config.GetValue<string>("Supermarket:OpeningHours") ?? "8am - 10:30pm";

        //        // Convert mm to PDF points
        //        const double mmToPoint = 2.83465;
        //        double width = 80 * mmToPoint;   // 80mm paper width

        //        // Compute a reasonable dynamic height so the receipt doesn't become too long.
        //        // Base header + item lines + footer. Capped to a realistic max length.
        //        int itemCount = checkout?.SubData?.Count ?? 0;
        //        double headerHeight = 70;               // points for store name, address, phone, open days/hours, date, title and header
        //        double perItemHeight = 10;              // each item line height in points
        //        double totalsAndFooterHeight = 60;      // space for line, total, issuer, margins
        //        double neededHeight = headerHeight + (itemCount * perItemHeight) + totalsAndFooterHeight;

        //        double minHeight = 80 * mmToPoint;     // ~80mm minimum
        //        double maxHeight = 150 * mmToPoint;    // ~150mm maximum to keep receipt realistic

        //        double height = Math.Max(minHeight, Math.Min(maxHeight, neededHeight));

        //        using (var ms = new MemoryStream())
        //        {
        //            var document = new PdfDocument();
        //            var page = document.AddPage();
        //            page.Width = width;
        //            page.Height = height;

        //            var gfx = XGraphics.FromPdfPage(page);
        //            var fontTitle = new XFont("Verdana", 10, XFontStyle.Bold);
        //            var font = new XFont("Verdana", 8, XFontStyle.Regular);
        //            var fontSmall = new XFont("Verdana", 7, XFontStyle.Regular);

        //            double y = 10;
        //            // Supermarket Name (centered)
        //            gfx.DrawString(storeName, fontTitle, XBrushes.Black,
        //                new XRect(0, y, page.Width, 12), XStringFormats.TopCenter);
        //            y += 12;

        //            // Address (centered)
        //            gfx.DrawString(storeAddress, font, XBrushes.Black,
        //                new XRect(0, y, page.Width, 10), XStringFormats.TopCenter);
        //            y += 10;

        //            // Phone (centered)
        //            gfx.DrawString(storePhone, font, XBrushes.Black,
        //                new XRect(0, y, page.Width, 10), XStringFormats.TopCenter);
        //            y += 10;

        //            // Opening days (centered, small)
        //            gfx.DrawString(storeOpenDays, fontSmall, XBrushes.Black,
        //                new XRect(0, y, page.Width, 8), XStringFormats.TopCenter);
        //            y += 8;

        //            // Opening hours (centered, small)
        //            gfx.DrawString(storeHours, fontSmall, XBrushes.Black,
        //                new XRect(0, y, page.Width, 8), XStringFormats.TopCenter);
        //            y += 10;

        //            // Date and Time
        //            string dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //            gfx.DrawString($"Date: {dateTime}", fontSmall, XBrushes.Black,
        //                new XRect(0, y, page.Width, 8), XStringFormats.TopCenter);
        //            y += 10;

        //            // Receipt title
        //            gfx.DrawString("RECEIPT", fontTitle, XBrushes.Black,
        //                new XRect(0, y, page.Width, 12), XStringFormats.TopCenter);
        //            y += 14;

        //            // Table header
        //            gfx.DrawString("Qty  Product           Amount", fontSmall, XBrushes.Black,
        //                new XRect(5, y, page.Width - 10, 8), XStringFormats.TopLeft);
        //            y += 10;

        //            decimal total = 0;
        //            if (checkout?.SubData != null)
        //            {
        //                foreach (var item in checkout.SubData)
        //                {
        //                    // Format: 2 X Drinks   1,300
        //                    string line = $"{item.Quantity} X {item.ProductName}".PadRight(18);
        //                    string amount = $"{item.FinalPrice:N0}";
        //                    gfx.DrawString(line, fontSmall, XBrushes.Black,
        //                        new XRect(5, y, 60, 8), XStringFormats.TopLeft);
        //                    gfx.DrawString(amount, fontSmall, XBrushes.Black,
        //                        new XRect(page.Width - 45, y, 40, 8), XStringFormats.TopRight);
        //                    y += perItemHeight;
        //                    total += item.FinalPrice;
        //                }
        //            }

        //            y += 6;
        //            gfx.DrawLine(XPens.Black, 5, y, page.Width - 5, y);
        //            y += 4;

        //            // Total
        //            gfx.DrawString("TOTAL", fontTitle, XBrushes.Black,
        //                new XRect(5, y, 60, 10), XStringFormats.TopLeft);
        //            gfx.DrawString($"{total:N2}", fontTitle, XBrushes.Black,
        //                new XRect(page.Width - 45, y, 40, 10), XStringFormats.TopRight);
        //            y += 14;

        //            // Issuer at the bottom (keep near bottom but not outside page)
        //            double issuerY = Math.Min(page.Height - 20, y + 6);
        //            gfx.DrawString($"Issuer: {checkout.LoggedInUser}", fontSmall, XBrushes.Black,
        //                new XRect(5, issuerY, page.Width - 10, 8), XStringFormats.TopLeft);

        //            document.Save(ms, false);
        //            return ms.ToArray();
        //        }
        //    }
        //    catch
        //    {
        //        // Bubble up to caller who will return a proper 500
        //        throw;
        //    }
        //}
        private byte[] GenerateReceiptPdf(Checkout checkout)
        {
            try
            {
                string storeName = _config.GetValue<string>("Supermarket:Name") ?? "Supermarket Name";
                string storeAddress = _config.GetValue<string>("Supermarket:Address") ?? "123 Main St, City";
                string storePhone = _config.GetValue<string>("Supermarket:Phone") ?? "Tel: 123-456-7890";
                string storeOpenDays = _config.GetValue<string>("Supermarket:OpeningDays") ?? "Mon-Sat";
                string storeHours = _config.GetValue<string>("Supermarket:OpeningHours") ?? "8am - 10:30pm";

                // Convert mm to PDF points
                const double mmToPoint = 2.83465;
                double paperWidth = 80 * mmToPoint;   // 80mm paper width

                // Layout settings
                const double leftMargin = 8;
                const double rightMargin = 8;
                double contentWidth = paperWidth - leftMargin - rightMargin;

                var fontTitle = new XFont("Verdana", 10, XFontStyle.Bold);
                var font = new XFont("Verdana", 8, XFontStyle.Regular);
                var fontSmall = new XFont("Verdana", 7, XFontStyle.Regular);

                // Helper to wrap text to fit a given width
                List<string> WrapText(XGraphics gfx, string text, XFont f, double maxWidth)
                {
                    var lines = new List<string>();
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        return lines;
                    }

                    var words = text.Split(' ');
                    var current = new StringBuilder();

                    foreach (var w in words)
                    {
                        var test = current.Length == 0 ? w : current + " " + w;
                        var size = gfx.MeasureString(test, f);
                        if (size.Width <= maxWidth)
                        {
                            if (current.Length > 0) current.Append(' ');
                            current.Append(w);
                        }
                        else
                        {
                            if (current.Length > 0)
                            {
                                lines.Add(current.ToString());
                                current.Clear();
                            }

                            // If a single word is longer than maxWidth, we must split the word.
                            var word = w;
                            while (gfx.MeasureString(word, f).Width > maxWidth && word.Length > 1)
                            {
                                int fitChars = word.Length;
                                // Binary-ish search for how many chars fit
                                int low = 0, high = word.Length;
                                while (low < high)
                                {
                                    int mid = (low + high + 1) / 2;
                                    var sub = word.Substring(0, mid);
                                    if (gfx.MeasureString(sub + "-", f).Width <= maxWidth) low = mid;
                                    else high = mid - 1;
                                }

                                if (low == 0) low = 1;
                                lines.Add(word.Substring(0, low) + "-");
                                word = word.Substring(low);
                            }

                            if (!string.IsNullOrEmpty(word))
                                current.Append(word);
                        }
                    }

                    if (current.Length > 0)
                        lines.Add(current.ToString());

                    return lines;
                }

                // Estimate required height: header + items + totals + footer
                int itemCount = checkout?.SubData?.Count ?? 0;
                // We'll recalc exact height after measuring wrapped lines, but start with a baseline
                double baseHeader = 12 + 8 + 8 + 6 + 6 + 10 + 14 + 10; // approximated sizes of header parts
                double baseFooter = 30;
                double estimatedPerItem = 10;
                double estimatedHeight = (baseHeader + baseFooter + (itemCount * estimatedPerItem));

                double minHeight = 80 * mmToPoint;     // ~80mm minimum
                double maxHeight = 150 * mmToPoint;    // ~150mm maximum

                double height = Math.Max(minHeight, Math.Min(maxHeight, estimatedHeight));

                using (var ms = new MemoryStream())
                {
                    var document = new PdfDocument();
                    var page = document.AddPage();
                    page.Width = paperWidth;
                    page.Height = height;

                    var gfx = XGraphics.FromPdfPage(page);

                    double y = 10;

                    // Draw store name centered
                    var storeNameSize = gfx.MeasureString(storeName, fontTitle);
                    gfx.DrawString(storeName, fontTitle, XBrushes.Black,
                        new XRect(0, y, page.Width, storeNameSize.Height), XStringFormats.TopCenter);
                    y += storeNameSize.Height + 4;

                    // Wrap the address so it never overflows
                    var addressLines = WrapText(gfx, storeAddress, font, contentWidth);
                    foreach (var line in addressLines)
                    {
                        var lineSize = gfx.MeasureString(line, font);
                        gfx.DrawString(line, font, XBrushes.Black,
                            new XRect(leftMargin, y, contentWidth, lineSize.Height), XStringFormats.TopCenter);
                        y += lineSize.Height + 2;
                    }

                    // Phone
                    var phoneSize = gfx.MeasureString(storePhone, font);
                    gfx.DrawString(storePhone, font, XBrushes.Black,
                        new XRect(0, y, page.Width, phoneSize.Height), XStringFormats.TopCenter);
                    y += phoneSize.Height + 4;

                    // Opening days and hours
                    var openDaysSize = gfx.MeasureString(storeOpenDays, fontSmall);
                    gfx.DrawString(storeOpenDays, fontSmall, XBrushes.Black,
                        new XRect(0, y, page.Width, openDaysSize.Height), XStringFormats.TopCenter);
                    y += openDaysSize.Height + 2;

                    var hoursSize = gfx.MeasureString(storeHours, fontSmall);
                    gfx.DrawString(storeHours, fontSmall, XBrushes.Black,
                        new XRect(0, y, page.Width, hoursSize.Height), XStringFormats.TopCenter);
                    y += hoursSize.Height + 6;

                    // Date and time
                    string dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    var dtSize = gfx.MeasureString($"Date: {dateTime}", fontSmall);
                    gfx.DrawString($"Date: {dateTime}", fontSmall, XBrushes.Black,
                        new XRect(0, y, page.Width, dtSize.Height), XStringFormats.TopCenter);
                    y += dtSize.Height + 8;

                    // Title
                    var titleSize = gfx.MeasureString("RECEIPT", fontTitle);
                    gfx.DrawString("RECEIPT", fontTitle, XBrushes.Black,
                        new XRect(0, y, page.Width, titleSize.Height), XStringFormats.TopCenter);
                    y += titleSize.Height + 6;

                    // Table header
                    gfx.DrawString("Qty", fontSmall, XBrushes.Black, new XRect(leftMargin, y, 30, 10), XStringFormats.TopLeft);
                    gfx.DrawString("Product", fontSmall, XBrushes.Black, new XRect(leftMargin + 30, y, contentWidth - 30 - 45, 10), XStringFormats.TopLeft);
                    gfx.DrawString("Amount", fontSmall, XBrushes.Black, new XRect(page.Width - rightMargin - 45, y, 45, 10), XStringFormats.TopRight);
                    y += gfx.MeasureString("Q", fontSmall).Height + 6;

                    decimal total = 0;

                    // For each item, wrap product name to product column width
                    double qtyColWidth = 30;
                    double amountColWidth = 45;
                    double productColWidth = contentWidth - qtyColWidth - amountColWidth;

                    if (checkout?.SubData != null)
                    {
                        foreach (var item in checkout.SubData)
                        {
                            string productText = $"{item.Quantity} X {item.ProductName}";
                            string qtyText = $"{item.Quantity} X";
                            string amountText = $"{item.FinalPrice:N2}";

                            // We will draw qty in left column, product wrapped in middle, amount aligned right
                            var productLines = WrapText(gfx, item.ProductName, fontSmall, productColWidth);

                            // If WrapText returns empty (e.g., null product), ensure at least one line
                            if (productLines.Count == 0) productLines.Add(string.Empty);

                            // Draw first line with qty and amount
                            gfx.DrawString(qtyText, fontSmall, XBrushes.Black,
                                new XRect(leftMargin, y, qtyColWidth, gfx.MeasureString(qtyText, fontSmall).Height), XStringFormats.TopLeft);

                            gfx.DrawString(productLines[0], fontSmall, XBrushes.Black,
                                new XRect(leftMargin + qtyColWidth, y, productColWidth, gfx.MeasureString(productLines[0], fontSmall).Height), XStringFormats.TopLeft);

                            gfx.DrawString(amountText, fontSmall, XBrushes.Black,
                                new XRect(page.Width - rightMargin - amountColWidth, y, amountColWidth, gfx.MeasureString(amountText, fontSmall).Height), XStringFormats.TopRight);

                            y += gfx.MeasureString(productLines[0], fontSmall).Height + 4;

                            // Draw remaining wrapped lines (product only, indented)
                            for (int i = 1; i < productLines.Count; i++)
                            {
                                var pl = productLines[i];
                                gfx.DrawString(pl, fontSmall, XBrushes.Black,
                                    new XRect(leftMargin + qtyColWidth, y, productColWidth, gfx.MeasureString(pl, fontSmall).Height), XStringFormats.TopLeft);
                                y += gfx.MeasureString(pl, fontSmall).Height + 4;
                            }

                            total += item.FinalPrice;
                        }
                    }

                    y += 4;
                    // Separator line
                    gfx.DrawLine(XPens.Black, leftMargin, y, page.Width - rightMargin, y);
                    y += 6;

                    // Total line
                    var totalLabelSize = gfx.MeasureString("TOTAL", fontTitle);
                    gfx.DrawString("TOTAL", fontTitle, XBrushes.Black,
                        new XRect(leftMargin, y, page.Width - leftMargin - rightMargin - amountColWidth, totalLabelSize.Height), XStringFormats.TopLeft);
                    gfx.DrawString($"{total:N2}", fontTitle, XBrushes.Black,
                        new XRect(page.Width - rightMargin - amountColWidth, y, amountColWidth, totalLabelSize.Height), XStringFormats.TopRight);
                    y += totalLabelSize.Height + 8;

                    // Issuer at the bottom (ensure it doesn't flow off the page)
                    double issuerY = Math.Min(page.Height - 20, y);
                    gfx.DrawString($"Issuer: {checkout.LoggedInUser}", fontSmall, XBrushes.Black,
                        new XRect(leftMargin, issuerY, contentWidth, gfx.MeasureString(checkout.LoggedInUser ?? string.Empty, fontSmall).Height), XStringFormats.TopLeft);

                    document.Save(ms, false);
                    return ms.ToArray();
                }
            }
            catch
            {
                // Bubble up to caller who will return a proper 500
                throw;
            }
        }
    }
}



