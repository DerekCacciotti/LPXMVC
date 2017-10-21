using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LPX.Models
{
    public class User
    {
        public int ID { get; set; }
     
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string DateCreated { get; set; }
        public bool PaidOnTime { get; set; }
        public string BillingAddress { get; set; }
        public string City { get; set; }
		public string State { get; set; }
		public string Zip { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        public string BusinessName { get; set; }
      

    }
}