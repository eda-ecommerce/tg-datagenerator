namespace eCommerceDataGenerator.Models;

public class OfferingWithQuantity
{
    public int Quantity { get; set; }
    
    public Guid OfferingId { get; set; }
    
    public float TotalPrice { get; set; }
}