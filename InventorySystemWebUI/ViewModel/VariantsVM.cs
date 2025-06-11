namespace InventorySystemWebUI.ViewModel;

public class VariantsVM
{
    public int Id { get; set; } // Primary Key
    public string Name { get; set; } // e.g., Color, Size, Volume
    public string Value { get; set; } // e.g., Red, Large, 500ml
    public int ProductId { get; set; } // Foreign Key linking to Product
    public ProductVM Product { get; set; }
}
