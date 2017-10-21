using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LPX.Models
{
    public class Suppiler
    {
        [Key]
        public int ID { get; set; }
        public string SupplierName { get; set; }
        public string SupplierEmail { get; set; }
        public int UserID { get; set; }
        public string Username { get; set; }
    }
}
