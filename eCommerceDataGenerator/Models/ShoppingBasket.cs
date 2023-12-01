namespace eCommerceDataGenerator.Models;

public class ShoppingBasket
{
    public Guid ShoppingBasketId { get; set; }
    public Guid CustomerId { get; set; }
    //public List<KeyValuePair<int, Offering>> items { get; set; }
    public List<OfferingWithQuantity> items { get; set; }
}