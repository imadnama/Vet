namespace ClinicVets.Core.Models;

public class Medicine
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    /// <summary>Price must be positive.</summary>
    public decimal Price { get; set; }

    /// <summary>Quantity must be a non-negative integer.</summary>
    public int Quantity { get; set; }
}
