namespace eCommerceDataGenerator.Models;

public class ShoppingBasket
{
    public Guid ShoppingBasketId { get; set; }
    public Guid CustomerId { get; set; }
    
    public float TotalPrice { get; set; }
    
    public int TotalItemQuantity { get; set; }
    public List<OfferingWithQuantity> Items { get; set; }
}