namespace InventorySystemWebUI.ViewModel
{
    public class ProductVM
    {
        public int? Id { get; set; }
        public string ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public int CategoryId { get; set; }
        public string SKU { get; set; }
        public string BarCodeNumber { get; set; }
        public decimal? Discount { get; set; } = 0.00m;
        public decimal Price { get; set; } = 0.00m;
        public List<VariantsVM> Variants { get; set; } = new List<VariantsVM>();
    }
}
