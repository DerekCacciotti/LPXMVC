using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using LPX.Models;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LPX.Controllers
{
    public class SuppliersController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View("fulfillorder");
        }



        [HttpPost]
        public JsonResult GetOrderDeatils(Order order)
        {

            using (var context = new LPXContext())
            {
                var dataset = context.Orders.Where(p => p.OrderNumber == order.OrderNumber && p.Suppliers.Contains(order.Suppliers)).Select(p => new Order
                {
                    OrderNumber = p.OrderNumber,
                    OrderDetails = p.OrderDetails,
                    //OrderDate = p.OrderDate,
                    Suppliers = p.Suppliers,




                }).ToList();



                if (dataset != null)
                {
                    foreach (Order ord in dataset)
                    {
                        Debug.WriteLine(ord.OrderDetails);
                        HttpContext.Session.SetString("order", ord.OrderDetails);
                        HttpContext.Session.SetString("ordernumber", ord.OrderNumber);
                        //HttpContext.Session.SetString("orderdate", ord.OrderDate.ToString());
                        HttpContext.Session.SetString("suppliers", ord.Suppliers);

                    }
                }

                return Json("");








            }
        }


        public IActionResult ShowDetails()
        {
            int amount = 0;
            List<OrderData> orderslist = new List<OrderData>();
            List<OrderData> finalorderlist = new List<OrderData>();
            JObject jsonobject = JObject.Parse(HttpContext.Session.GetString("order"));
            foreach (JProperty prop in (JToken)jsonobject)
            {

                if (prop.Name == "data")
                {
                    //Debug.WriteLine(prop.Value.ToString());


                    JArray dataarray = JArray.Parse(prop.Value.ToString());


                    foreach (JObject o in dataarray.Children<JObject>())
                    {
                        OrderData or = new OrderData();
                        foreach (JProperty p in o.Properties())
                        {
                            string propname = p.Name;


                            if (propname.Equals("ProductName"))
                            {
                                or.ProductName = p.Value.ToString();

                            }
                            else if (propname.Equals("Brand"))
                            {
                                or.Brand = p.Value.ToString();
                            }

                            else if (propname.Equals("PackSize"))
                            {
                                or.PackSize = p.Value.ToString();

                            }

                            string propvalue = p.Value.ToString();


                            Debug.WriteLine(propname + " " + propvalue);





                        }
                        orderslist.Add(or);

                    }

                }
                else if (prop.Name == "amounts")
                {
                    //JArray amountsarray = JArray.Parse(prop.Value.ToString());
                    var amountarray = prop.Value.ToString();


                    if (amountarray.Contains(','))
                    {
                        var orderlistarray = orderslist.ToArray();
                        string[] amounts = amountarray.Split(',');

                        for (int i = 0; i < orderlistarray.Length; i++)
                        {
                            OrderData currentdata = orderlistarray[i];

                            currentdata.Amount = Convert.ToInt32(amounts[i]);
                            finalorderlist.Add(currentdata);

                        }

                        //foreach(string strvalue in amounts)
                        //{
                        //     amount = Convert.ToInt32(strvalue);
                        //    foreach(OrderData data in orderslist)
                        //    {
                        //        data.Amount = amount;
                        //        finalorderlist.Add(data);

                        //    }



                        //}





                        orderslist = null;


                    }

                    else
                    {
                        amount = Convert.ToInt32(amountarray);

                        foreach (OrderData data in orderslist)
                        {
                            data.Amount = amount;
                            finalorderlist.Add(data);
                        }

                        orderslist = null;
                        amount = 0;

                    }




                    Debug.WriteLine(prop.Value.ToString());






                }

            }






            //foreach (KeyValuePair<string, JToken> kvp in jsonobject)
            //{
            //    if(kvp.Key == "data")
            //    {
            //        Debug.WriteLine(kvp.Value);




            //        string[] orderparts = kvp.Value.ToString().Split(',');

            //        foreach(string part in orderparts)
            //        {
            //            Debug.WriteLine(part);



            //        }




            //    }
            //}














            return View("details", finalorderlist);
        }


        public string[] createstringarray(string str)
        {
            return new[] { str };
        }

        public string CleanJson(string data)
        {
            string editestring = data.Replace("\n", "");

            return editestring;
        }

        [HttpPost]
        public IActionResult CreateFulfilledOrder(FulfilledOrder fm)
        {
            var orderdate = HttpContext.Session.GetString("orderdate");
            var ordernumber = HttpContext.Session.GetString("ordernumber");
            var suppliers = HttpContext.Session.GetString("suppliers");

            using (var context = new LPXContext())
            {
                var furfilledorder = new FulfilledOrder();
                furfilledorder.OrderNumber = fm.OrderNumber;
                furfilledorder.OrderDate = orderdate;
                furfilledorder.RespondingSupplier = fm.RespondingSupplier;
                furfilledorder.Orderdata = fm.Orderdata;
                context.FulfilledOrders.Add(fm);
                context.SaveChanges();
                SendEmailbacktocustomer(furfilledorder);
                return Json(furfilledorder);
            }
        }



        private void SendEmailbacktocustomer(FulfilledOrder fo)
        {
            using (var context = new LPXContext())
            {
                var originalorder = context.Orders.First(a => a.OrderNumber == fo.OrderNumber);
                string customeremail = originalorder.CustomerEmail;
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("LPX Orders", "orders@lpxco.com"));
                message.To.Add(new MailboxAddress("LPX Customer", customeremail));
                message.Subject = "LPX Order " + fo.OrderNumber;
                var bodybuilder = new BodyBuilder();
                bodybuilder.HtmlBody = "<h2>Your LPX has been fulfilled</h2>" + "\n" + "<p>Click here to procure your products</p>";
                message.Body = bodybuilder.ToMessageBody();

                using (var clinet = new SmtpClient())
                {
                    clinet.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    clinet.Connect("smtp.gmail.com", 587);
                    clinet.AuthenticationMechanisms.Remove("XOAUTH2");
                    clinet.Authenticate("orders@lpxco.com", "Asking12");
                    clinet.Send(message);
                    clinet.Disconnect(true);
                }
            }





        }
    }
}



