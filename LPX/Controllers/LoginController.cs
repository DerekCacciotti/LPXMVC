using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LPX.Models;
using CryptoHelper;
using System.Diagnostics;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LPX.Controllers
{
    public class LoginController : Controller
    {
        // GET: /<controller>/
        public IActionResult Signin()
        {
            return View();
        }


        [HttpPost]
        public IActionResult LoginUser(User user)
        {

            if(ModelState.IsValid)
            {
				using (var context = new LPXContext())
				{
					var returningvisitor = new User();
					returningvisitor.UserName = user.UserName;
					returningvisitor.Password = user.Password;

					string storedpassword = context.Users.Where(u => u.UserName == returningvisitor.UserName).Select(u => u.Password).FirstOrDefault();

					if (!Crypto.VerifyHashedPassword(storedpassword, returningvisitor.Password))
					{
						returningvisitor = null;
						return RedirectToAction("Signin");
					}
					else
					{
						HttpContext.Session.SetObjectAsJson("CurrentUser", returningvisitor);
						Debug.WriteLine("logged in");
                        return Redirect("/admin/dashboard");
					}




				}
            }
            else
            {
                return RedirectToAction("Signin");
            }

         



           
        }



    }
}
