using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerceDataGenerator.Models;

public class Order
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public DateOnly OrderDate { get; set; }
    public bool Status { get; set; }
    public List<OfferingWithQuantity> Items { get; set; }
    public float TotalPrice {  get; set; }
}
