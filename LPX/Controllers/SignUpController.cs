using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using LPX.Models;
using CryptoHelper;
using System.Diagnostics;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LPX.Controllers
{
    public class SignUpController : Controller
    {
        [HttpGet]
        // GET: /<controller>/
        public IActionResult CreateAccount()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Register(User newuser)
        {
            using (var context = new LPXContext())
            {
                var usertoberegistered = new User();
                usertoberegistered.FirstName = newuser.FirstName;
                usertoberegistered.LastName = newuser.LastName;
                usertoberegistered.BillingAddress = newuser.BillingAddress;
                usertoberegistered.EmailAddress = newuser.EmailAddress;
                usertoberegistered.City = newuser.City;
                // usertoberegistered.DateCreated = DateTime.Now.ToString();
                usertoberegistered.State = newuser.State;
                usertoberegistered.Zip = newuser.Zip;
                usertoberegistered.UserName = newuser.UserName;
                usertoberegistered.Password = Crypto.HashPassword(newuser.Password);
                usertoberegistered.BusinessName = newuser.BusinessName;
                context.Users.Add(usertoberegistered);
                context.SaveChanges();
                HttpContext.Session.SetObjectAsJson("CurrentUser", usertoberegistered);
                return RedirectToAction("RegisterSuppliers");


            }

           

        }

        [HttpGet]
        public IActionResult RegisterSuppliers()
        {
			var currentuser = HttpContext.Session.GetObjectFromJson<User>("CurrentUser");
            Debug.WriteLine(currentuser);
            return View("registersuppliers");
           




        }

        [HttpPost]
        public IActionResult CommitSuppliers(Suppiler supplier)
        {
            using(var context = new LPXContext())
            {
                var currentuser = HttpContext.Session.GetObjectFromJson<User>("CurrentUser");
                var newsupplier = new Suppiler();
                newsupplier.SupplierName = supplier.SupplierName;
                newsupplier.SupplierEmail = supplier.SupplierEmail;
                newsupplier.UserID = currentuser.ID;
                context.Suppliers.Add(newsupplier);
                context.SaveChanges();
                return RedirectToAction("Success");
            }
        }

        [HttpGet]
        public IActionResult Success()
        {
            return View("success");
        }

       
    }
}
