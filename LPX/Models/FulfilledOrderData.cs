using System;
namespace LPX.Models
{
    public class FulfilledOrderData
    {
        public string ProductName { get; set; }
        public string Brand { get; set; }
        public string PackSize { get; set; }
        public int Amount { get; set; }
        public string SupplierName { get; set; }
        public double SellerPrice { get; set; }
    }
}
