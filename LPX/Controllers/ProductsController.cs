using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LPX.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using X.PagedList.Mvc.Core;
using X.PagedList;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LPX.Controllers
{
    public class ProductsController : Controller
    {
        bool issearching = false;

        // GET: /<controller>/
        public object Search(int? page)
        {

            using (var context = new LPXContext())
            {
                var fooditems = context.FoodSource;
                var pagenumber = page ?? 1;
                var onepageofproducts = fooditems.ToPagedList(pagenumber, 25);
                ViewBag.OnePageOfProducts = onepageofproducts;
                return View();
            }










        }


        [HttpGet]
        public object SearchbyItem(string searchstr, int? page)
        {
            using (var context = new LPXContext())
            {
                issearching = true;

                ViewBag.isSearching = issearching;

                string querystring = Request.QueryString.Value;
                string term = querystring.Replace("?SearchString=", "").ToUpper();
                Debug.WriteLine(term);
                ViewBag.Term = term;





                var dataset = context.FoodSource.Where(p => p.ProductName.Contains(term) || p.Brand.Contains(term)).Select(p => new FoodModel
                {
                    ID = p.ID,
                    ProductName = p.ProductName,
                    Brand = p.Brand,
                    PackSize = p.PackSize
                }).ToList();

                if (ViewBag.OnePageOfProducts != null)
                {
                    ViewBag.OnePageOfProducts = null;
                }

                var pagenumber = page ?? 1;
                var onepageofproducts = dataset;
                ViewBag.OnePageOfProducts = onepageofproducts;
                return View("search");
            }
        }



        [HttpGet]
        public object Details(int id)
        {
            using (var context = new LPXContext())
            {
                string querystring = Request.QueryString.Value;
                string productnumberstr = querystring.Replace("?id=", "");
                int productnum = Convert.ToInt32(productnumberstr);
                Debug.WriteLine(productnumberstr);
                Debug.WriteLine(productnum);

                var dataset = context.FoodSource.Where(p => p.ID == productnum).Select(p => new FoodModel
                {
                    ID = p.ID,
                    ProductName = p.ProductName,
                    Brand = p.Brand,
                    PackSize = p.PackSize
                }).FirstOrDefault();

                return View("details", dataset);
            }



        }

        [HttpGet]
        public IActionResult Cart(FoodModel fm)
        {
            Debug.WriteLine(fm);

            var savedfoodmodel = new FoodModel();
            savedfoodmodel.ID = fm.ID;
            savedfoodmodel.ProductName = fm.ProductName;
            savedfoodmodel.PackSize = fm.PackSize;
            savedfoodmodel.Brand = fm.Brand;
            savedfoodmodel.Supplier = fm.Supplier;
            HttpContext.Session.SetObjectAsJson("saveditem", savedfoodmodel);



            return Json(fm);
        }


        [HttpGet]
        public IActionResult ViewCart()
        {
            List<FoodModel> itemslist = new List<FoodModel>();
            if (HttpContext.Session.GetString("cartjson") != null)
            {
                itemslist = JsonConvert.DeserializeObject<List<FoodModel>>(HttpContext.Session.GetString("cartjson"));
            }

            // var currentcart = HttpContext.Session.GetObjectFromJson<FoodModel>("currentcart");





            var foodmodel = HttpContext.Session.GetObjectFromJson<FoodModel>("saveditem");
            itemslist.Add(foodmodel);
            var cartjson = JsonConvert.SerializeObject(itemslist);
            HttpContext.Session.SetString("cartjson", cartjson);
            Debug.WriteLine(foodmodel);
            Debug.WriteLine(cartjson);



            var currentuser = HttpContext.Session.GetObjectFromJson<User>("CurrentUser");
            Debug.WriteLine(currentuser.UserName);


            return View("cart", itemslist);







        }


        [HttpGet]
        public IActionResult SaveCart()
        {
            // save current model in a session 



            return RedirectToAction("Search");

        }

        [HttpPost]

        public IActionResult getsuppliers()
        {
            var currentuser = HttpContext.Session.GetObjectFromJson<User>("CurrentUser");

            using (var context = new LPXContext())
            {

                var dataset = context.Suppliers.Where(p => p.Username == currentuser.UserName).Select(p => new Suppiler
                {
                    SupplierName = p.SupplierName,
                    SupplierEmail = p.SupplierEmail,
                    Username = currentuser.UserName
                }).ToList();

                if (ViewBag.Suppliers != null)
                {
                    ViewBag.Suppliers = null;

                }
                var suppliers = dataset;

                ViewBag.Suppliers = suppliers;

                return Json(suppliers);


            }









        }

        [HttpPost]
        public IActionResult CreateOrder(Order order)
        {
			var currentuser = HttpContext.Session.GetObjectFromJson<User>("CurrentUser");


            using (var context = new LPXContext())
            {

                var dborder = new Order();
                dborder.OrderNumber = order.OrderNumber;
                dborder.OrderDate = DateTime.Now.ToString();
                dborder.PlacedBy = currentuser.UserName;
                dborder.OrderDetails = order.OrderDetails;
                dborder.RestaurantName = currentuser.BusinessName;
                dborder.Suppliers = order.Suppliers;

                HttpContext.Session.SetObjectAsJson("finalorder", dborder);
                return RedirectToAction("Savetodatabase");
              

            }


			

		}


        [HttpGet]
        public IActionResult Savetodatabase()
        {
            using(var context = new LPXContext())
            {
				var orderobject = HttpContext.Session.GetObjectFromJson<Order>("finalorder");
                context.Orders.Add(orderobject);
                context.SaveChanges();
                SendEmail(orderobject);
                Debug.WriteLine(Url.ActionContext);
                return Json(orderobject);

            }
           


        }


        private void SendEmail(Order order)
        {
            if (order.Suppliers.Count() > 1)
            {
                Debug.WriteLine(order.Suppliers);
                string[] emails = order.Suppliers.Split(',');

                foreach(string emailaddress in emails)
                {
                    Debug.WriteLine(emailaddress);
					var currentuser = HttpContext.Session.GetObjectFromJson<User>("CurrentUser");
					var message = new MimeMessage();
					message.From.Add(new MailboxAddress("LPX Orders", "orders@lpxco.com"));
                    message.To.Add(new MailboxAddress("Suppliers", emailaddress));
					message.Subject = "LPX Order: " + order.OrderNumber;

					var bodyBuilder = new BodyBuilder();
					bodyBuilder.HtmlBody = String.Format("<h2>Hello You have a new LPX Order {0}</h2>" + "\n" + "<p>this order was placed by {1}.</p>" + "\n " + @"<a href=""/suppliers/"">Please clcik here to fulfill the order</a>", order.OrderNumber, order.PlacedBy);



					message.Body = bodyBuilder.ToMessageBody();

					using (var client = new SmtpClient())
					{
						client.ServerCertificateValidationCallback = (s, c, h, e) => true;
						client.Connect("smtp.gmail.com", 587);



						// Note: since we don't have an OAuth2 token, disable
						// the XOAUTH2 authentication mechanism.
						client.AuthenticationMechanisms.Remove("XOAUTH2");

						// Note: only needed if the SMTP server requires authentication
						client.Authenticate("orders@lpxco.com", "Asking12");

						client.Send(message);
						client.Disconnect(true);
					}
				}

                }

            else
            {
				var currentuser = HttpContext.Session.GetObjectFromJson<User>("CurrentUser");
				var message = new MimeMessage();
				message.From.Add(new MailboxAddress("LPX Orders", "orders@lpxco.com"));
				message.To.Add(new MailboxAddress("Suppliers", order.Suppliers));
				message.Subject = "LPX Order: " + order.OrderNumber;

				var bodyBuilder = new BodyBuilder();
				bodyBuilder.HtmlBody = String.Format("<h2>Hello You have a new LPX Order {0}</h2>" + "\n" + "<p>this order was placed by {1}.</p>" + "\n " + @"<a href=""/suppliers/"">Please clcik here to fulfill the order</a>", order.OrderNumber, order.PlacedBy);



				message.Body = bodyBuilder.ToMessageBody();

				using (var client = new SmtpClient())
				{
					client.ServerCertificateValidationCallback = (s, c, h, e) => true;
					client.Connect("smtp.gmail.com", 587);



					// Note: since we don't have an OAuth2 token, disable
					// the XOAUTH2 authentication mechanism.
					client.AuthenticationMechanisms.Remove("XOAUTH2");

					// Note: only needed if the SMTP server requires authentication
					client.Authenticate("orders@lpxco.com", "Asking12");

					client.Send(message);
					client.Disconnect(true);
				}
            }

               
        }



       







    }


}
