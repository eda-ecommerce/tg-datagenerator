namespace eCommerceDataGenerator.Models;

public class ShoppingBasket
{
    public Guid shoppingBasketId { get; set; }
    public Guid customerId { get; set; }
    
    public float totalPrice { get; set; }
    
    public int totalItemQuantity { get; set; }
    public List<OfferingWithQuantity> shoppingBasketItems { get; set; }
}