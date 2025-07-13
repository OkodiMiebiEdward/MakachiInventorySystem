using System.ComponentModel.DataAnnotations;

namespace InventorySystemWebUI.ViewModel;

public class StockVM
{
    public int? Id { get; set; }
    public string ProductName { get; set; } = "";
    public int ProductId { get; set; }
    public int QuantityInStock { get; set; }
    public decimal CostUnitPrice { get; set; }
    public decimal SellingUnitPrice { get; set; }
    public decimal? Discount { get; set; }
    public DateTime? CreatedAt { get; set; } = DateTime.Now;
    public int CategoryId { get; set; }
    public string StockNumber { get; set; }
    public decimal FinalPrice { get; set; }
}
