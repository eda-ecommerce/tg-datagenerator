namespace eCommerceDataGenerator.Models;

public class ShoppingBasket
{
    public Guid ShoppingBasketId { get; set; }
    public Guid CustomerId { get; set; }
    public List<OfferingWithQuantity> Items { get; set; }
    public string Type { get; set; }
}