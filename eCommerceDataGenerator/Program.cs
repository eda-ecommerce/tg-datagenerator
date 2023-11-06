using System.Net;
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
string KAFKA_TOPIC = configuration.GetValue<string>("Kafka:Topic")!;
bool KAFKA_DELETE_TOPIC_FIRST = configuration.GetValue<bool>("Kafka:DeleteTopicFirst")!;
int AMOUNT_OF_USERS_TO_GENERATE = configuration.GetValue<int>("Kafka:AmountOfUsersToGenerate")!;


// Generate mock users
var mockUsers = new Faker<User>()
    .RuleFor(u => u.Firstname, (f, u) => f.Name.FirstName())
    .RuleFor(u => u.Lastname, (f, u) => f.Name.LastName())
    .RuleFor(u => u.Username, (f, u) => u.Firstname + u.Lastname);

var mockGeneratedUsers = mockUsers.Generate(AMOUNT_OF_USERS_TO_GENERATE);

if (KAFKA_DELETE_TOPIC_FIRST)
{
    // Delete topic
    var configAdmClient = new AdminClientConfig
    {
        BootstrapServers = KAFKA_BROKER
    };

    using var adminClient = new AdminClientBuilder(configAdmClient).Build();
    await adminClient.DeleteTopicsAsync(new string[] { KAFKA_TOPIC }, null);
}

// Produce messages
ProducerConfig configProducer = new ProducerConfig
{
    BootstrapServers = KAFKA_BROKER,
    ClientId = Dns.GetHostName()
};

using var producer = new ProducerBuilder<Null, string>(configProducer).Build();
foreach (var user in mockGeneratedUsers)
{
    var result = await producer.ProduceAsync(KAFKA_TOPIC, new Message<Null, string>
    {
        Value = JsonSerializer.Serialize<User>(user)
    });
    Console.WriteLine(JsonSerializer.Serialize<User>(user));

}
