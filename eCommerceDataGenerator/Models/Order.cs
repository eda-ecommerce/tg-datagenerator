using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerceDataGenerator.Models;

public class Order
{


    /// <summary>
    /// Durch [JsonIgnore] würde der Value nicht an Kafka gesendet werden
    /// </summary>
    //[JsonIgnore]
    public Guid OrderId { get; set; }
    
    /// <summary>
    /// Durch [JsonIgnore] würde der Value nicht an Kafka gesendet werden
    /// </summary>
    //[JsonIgnore]
    public DateOnly CreateDate { get; set; }

    /// <summary>
    /// Bsp: DieterMücke
    /// </summary>
    public Boolean Status{ get; set; }

    /// <summary>
    /// Bsp: DieterMücke
    /// </summary>
    public DateOnly PaymentDate {  get; set; }
}
