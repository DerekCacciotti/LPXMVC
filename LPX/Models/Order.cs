using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LPX.Models
{
    public class Order
    {
        [Key]
        public string OrderNumber { get; set; }
        public string OrderDate { get; set; }
        public string PlacedBy { get; set; }
        public string Suppliers { get; set; }
        public string OrderDetails { get; set; }
        public string RestaurantName { get; set; }
        public string CustomerEmail { get; set; }

        public Order()
        {
        }
    }
}
