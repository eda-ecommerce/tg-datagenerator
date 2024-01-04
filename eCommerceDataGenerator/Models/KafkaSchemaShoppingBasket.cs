namespace eCommerceDataGenerator.Models;

public class KafkaSchemaShoppingBasket
{
    public String Source { get; set; }
    public long Timestamp { get; set; }
    public string Operation { get; set; }
    public ShoppingBasket ShoppingBasket { get; set; }
}