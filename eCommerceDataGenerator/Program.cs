using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Bogus;
using Confluent.Kafka;
using eCommerceDataGenerator.Models;

// Read appsettings
IConfigurationRoot configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();



// Get appsettings and set as variable
string KAFKA_BROKER = configuration.GetValue<string>("Kafka:Broker")!;
string KAFKA_TOPIC1 = configuration.GetValue<string>("Kafka:Topic1")!;
string KAFKA_TOPIC2 = configuration.GetValue<string>("Kafka:Topic2")!;
bool KAFKA_DELETE_TOPIC_FIRST = configuration.GetValue<bool>("Kafka:DeleteTopicFirst")!;
int AMOUNT_OF_USERS_TO_GENERATE = configuration.GetValue<int>("Kafka:AmountOfUsersToGenerate")!;
bool createOrderOrShoppingBasket = configuration.GetValue<bool>("CreateOrderOrShoppingBasket")!;
Guid shoppingBasketId = Guid.NewGuid();
// Generate OfferingWithQuantity
var mockOfferingsWithQuantity = new Faker<OfferingWithQuantity>()
    .RuleFor(ow => ow.shoppingBasketId, f => shoppingBasketId)
    .RuleFor(ow => ow.quantity, f => f.Random.Int(1, 10))
    .RuleFor(ow => ow.offeringId, f => Guid.NewGuid())
    .RuleFor(o => o.totalPrice, f => f.Random.Float(1, 20))
    .RuleFor(o => o.itemState, f => "active");

// Generate mock shopping basket
var mockShoppingBasket = new Faker<ShoppingBasket>()
    .RuleFor(s => s.shoppingBasketId, f => shoppingBasketId)
    .RuleFor(s => s.customerId, f => Guid.NewGuid())
    .RuleFor(s => s.shoppingBasketItems, f => mockOfferingsWithQuantity.Generate(f.Random.Int(2, 7)).ToList())
    .RuleFor(ow => ow.totalItemQuantity, f => f.Random.Int(1, 10))
    .RuleFor(o => o.totalPrice, f => f.Random.Float(20, 150));

// Generate mock order
var mockOrder = new Faker<Order>()
    .RuleFor(o => o.OrderId, f => Guid.NewGuid())
    .RuleFor(o => o.CustomerId, f => Guid.NewGuid())
    .RuleFor(u => u.OrderDate, (f, o) => f.Date.BetweenDateOnly(
        new DateOnly(2003, 1, 1),
        new DateOnly(2023, 1, 3)))
    .RuleFor(o => o.Status, f => false) //for random true or false -> f.IndexFaker == 0 ? true : false)
    .RuleFor(o => o.Items, f => mockOfferingsWithQuantity.Generate(f.Random.Int(2, 7)).ToList())
    .RuleFor(o => o.TotalPrice, f => f.Random.Float(20, 150));


var mockKafkaSchemaShoppingBasket = new Faker<KafkaSchemaShoppingBasket>()
    .RuleFor(kssb => kssb.ShoppingBasket, mockShoppingBasket);

// // Generate mock users
// var mockUsers = new Faker<User>()
//     .RuleFor(u => u.Firstname, (f, u) => f.Name.FirstName())
//     .RuleFor(u => u.Lastname, (f, u) => f.Name.LastName())
//     .RuleFor(u => u.Username, (f, u) => u.Firstname + u.Lastname);

// var mockGeneratedUsers = mockUsers.Generate(AMOUNT_OF_USERS_TO_GENERATE);
var mockGeneratedShoppingBasket = mockKafkaSchemaShoppingBasket.Generate(1);
var mockGeneratedOrders = mockOrder.Generate(1);

if (KAFKA_DELETE_TOPIC_FIRST)
{
    // Delete topic
    var configAdmClient = new AdminClientConfig
    {
        BootstrapServers = KAFKA_BROKER
    };

    using var adminClient = new AdminClientBuilder(configAdmClient).Build();
    await adminClient.DeleteTopicsAsync(new string[] { KAFKA_TOPIC1, KAFKA_TOPIC2 }, null);
}

// Produce messages config
ProducerConfig configProducer = new ProducerConfig
{
    BootstrapServers = KAFKA_BROKER,
    ClientId = Dns.GetHostName()
};

// Create Kafka Header
var orderHeader = new Headers();
orderHeader.Add("source", Encoding.UTF8.GetBytes("order"));
orderHeader.Add("timestamp", Encoding.UTF8.GetBytes(new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString()));
orderHeader.Add("operation", Encoding.UTF8.GetBytes("created"));

var shoppingBasketHeader = new Headers();
shoppingBasketHeader.Add("source", Encoding.UTF8.GetBytes("shoppingBasket"));
shoppingBasketHeader.Add("timestamp", Encoding.UTF8.GetBytes(new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString()));
shoppingBasketHeader.Add("operation", Encoding.UTF8.GetBytes("created"));

// Produce user
// to create an shoppingbasket => appsatings -> createPaymentOrShoppingBasket = true
if (createOrderOrShoppingBasket)
{
    using var producer = new ProducerBuilder<Null, string>(configProducer).Build();
    foreach (var shoppingItem in mockGeneratedShoppingBasket)
    {
        var result = await producer.ProduceAsync(KAFKA_TOPIC2, new Message<Null, string>
        {
            Value = JsonSerializer.Serialize<ShoppingBasket>(shoppingItem.ShoppingBasket),
            Headers = shoppingBasketHeader
        });
        Console.WriteLine(JsonSerializer.Serialize<KafkaSchemaShoppingBasket>(shoppingItem));

    }
}

// Produce Order
// to create an order => appsatings -> createPaymentOrShoppingBasket = false
if (!createOrderOrShoppingBasket)
{
    using var producer1 = new ProducerBuilder<Null, string>(configProducer).Build();
    foreach (var order in mockGeneratedOrders)
    {
        var result = await producer1.ProduceAsync(KAFKA_TOPIC1, new Message<Null, string>
        {
            Value = JsonSerializer.Serialize<Order>(order),
            Headers = orderHeader
        });
        Console.WriteLine(JsonSerializer.Serialize<Order>(order));

    }
}

