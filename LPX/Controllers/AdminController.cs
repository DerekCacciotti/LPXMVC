using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LPX.Models;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LPX.Controllers
{
    public class AdminController : Controller
    {
        // GET: /<controller>/
        public IActionResult Dashboard()
        {
            var currentuser = HttpContext.Session.GetObjectFromJson<User>("CurrentUser");

            if(currentuser == null)
            {
                return Redirect("/Home/Index");
            }
            else
            {
                return View("dashboard");
            }


        }


        // view orders start here 

        public IActionResult ViewOrders()
        {
            return View("vieworders");
        }


        [HttpPost]
        public IActionResult RetreveOrder(Order or)
        {
            List<FulfilledOrderData> orderdatalist = new List<FulfilledOrderData>();
            using (var context = new LPXContext())
            {
                var dataset = context.FulfilledOrders.Where(x => x.OrderNumber == or.OrderNumber).Select(x => new FulfilledOrder
                {
                    OrderNumber = x.OrderNumber,
                    OrderDate = x.OrderDate,
                    RespondingSupplier = x.RespondingSupplier,
                    Orderdata = x.Orderdata,
                    Suppliers = x.Suppliers,
                    SupplierName = x.SupplierName
                                 




                }).ToList();



                foreach(var item in dataset)
                {
                    JArray orderdata = JArray.Parse(item.Orderdata);

                    Debug.WriteLine(orderdata.ToString());


                    foreach(JObject o in orderdata.Children<JObject>())
                    {
                        FulfilledOrderData fd = new FulfilledOrderData();
                        foreach(JProperty p in o.Properties())
                        {
                            string propname = p.Name;

                            if (propname.Equals("ProductName"))
                            {
                                fd.ProductName = p.Value.ToString();
                            }
                            else if (propname.Equals("Brand"))
                            {
                                fd.Brand = p.Value.ToString();

                            }
                            else if (propname.Equals("Amount"))
                            {
                                fd.Amount = Convert.ToInt32(p.Value.ToString());
                            }

                            else if (propname.Equals("PackSize"))
                            {
                                fd.PackSize = p.Value.ToString();
                            }
                            else
                            {
                                fd.SellerPrice = Convert.ToDouble(p.Value.ToString());
                                fd.SupplierName = item.SupplierName;
                            }

                        }

                        orderdatalist.Add(fd);
                    }



                }
                //            for (int i = 0; i < dataset.ToArray().Length; i++)
                //            {
                //                FulfilledOrderData fdata = new FulfilledOrderData();
                //                FulfilledOrder currentdataset = dataset.ToArray()[i];

                //                JArray jarray = JArray.Parse(currentdataset.Orderdata);
                //	foreach (var item in jarray.Children())
                //             {
                //    var props = item.Children<JProperty>();
                //    var productname = props.FirstOrDefault(x => x.Name == "ProductName");
                //    var brand = props.FirstOrDefault(x => x.Name == "Brand");
                //    var amount = props.FirstOrDefault(x => x.Name == "Amount");
                //    var packsize = props.FirstOrDefault(x => x.Name == "PackSize");
                //    var sellerprice = props.FirstOrDefault(x => x.Name == "SellerPrice");

                //		      fdata.ProductName = productname.Value.ToString();
                //		      fdata.Brand = brand.Value.ToString();
                //		      fdata.Amount = Convert.ToInt32(amount.Value.ToString());
                //		      fdata.PackSize = packsize.Value.ToString();
                //                    fdata.SellerPrice = Convert.ToDouble(sellerprice.Value.ToString());
                //                    fdata.SupplierName = currentdataset.RespondingSupplier;

                //		      orderdatalist.Add(fdata);


                //		    }





                //}


                //var uniqueitems = orderdatalist.GroupBy(x => x.SupplierName);
                //IEnumerable<FulfilledOrderData> sorteditems = uniqueitems.SelectMany(group => group);
                //// add distinct back if non disired results 
                //List<FulfilledOrderData> finalitems = sorteditems.ToList();

                //foreach (FulfilledOrderData fd in finalitems)
                //{
                //    Debug.WriteLine(fd.ProductName);
                //    Debug.WriteLine(fd.SupplierName);
                //}



                ////ViewBag.OrderItems = finalitems;

                //var finalitemsjson = JsonConvert.SerializeObject(finalitems);
                //var finalitemsjsonstring = finalitemsjson.ToString();
                //Debug.WriteLine(finalitemsjsonstring);

                //HttpContext.Session.SetString("finalitems", finalitemsjsonstring);

                var ordatalistjson = JsonConvert.SerializeObject(orderdatalist);

                var orderlistjsonasstr = ordatalistjson.ToString();
                HttpContext.Session.SetString("finalitems", orderlistjsonasstr);


                return Json(orderdatalist);
              
            }

			
        }

        [HttpGet]
        public IActionResult OrderDetails()
        {
            List<FulfilledOrderData> finalorder = new List<FulfilledOrderData>();
            if(HttpContext.Session.GetString("finalitems") != null)
            {
                finalorder = JsonConvert.DeserializeObject<List<FulfilledOrderData>>(HttpContext.Session.GetString("finalitems"));
                return View("orderdetails", finalorder);


            }
            else
            {
                return RedirectToAction("ViewOrders");
            }

          

        }
          
    }
}
