namespace eCommerceDataGenerator.Models;

public class OfferingWithQuantity
{
    public Guid shoppingBasketItemId { get; set; }
    public Guid shoppingBasketId { get; set; }
    public int quantity { get; set; }
    
    public Guid offeringId { get; set; }
    
    public float totalPrice { get; set; }
    
    public string itemState { get; set; }
}